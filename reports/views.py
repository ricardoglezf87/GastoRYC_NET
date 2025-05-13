from django.shortcuts import redirect, render
from django.urls import reverse
from GARCA.utils import add_breadcrumb, clear_breadcrumbs
from accounts.models import AccountKeyword
from async_tasks.tasks import recalculate_balances_after_date
from entries.models import Entry
from datetime import datetime, timedelta
from dateutil.relativedelta import relativedelta
from django.shortcuts import render, redirect
from django.http import JsonResponse
from django.views.decorators.http import require_POST
from django.db import transaction as db_transaction
from django.db.models import Count, Sum
from django.contrib import messages
from entries.models import Entry
from transactions.models import Transaction
from accounts.models import Account
from .forms import PeriodFilterForm 
from async_tasks.tasks import recalculate_balances_after_date 
from decimal import Decimal
from django.conf import settings
from django.db.models import Sum, F, DecimalField
from django.db.models.functions import Coalesce, Abs # Importar Abs
from django.core.cache import cache
import time
import json
import uuid
import logging

from transactions.models import Transaction
from .google_sheets_updater import update_google_sheet_with_data # Import the function
logger = logging.getLogger(__name__)


def recategorized_entries(request):
    form = PeriodFilterForm(request.GET or None)
    min_date = None
    affected_accounts = set()
    period_value = '' # Para el redirect

    if form.is_valid():
        start_date, end_date = form.get_date_range()
        period_value = form.cleaned_data.get('period', '') # Guardar el periodo validado

        # --- Lógica de filtrado por fecha ---
        if start_date and end_date:
            entries_in_period = Entry.objects.filter(date__gte=start_date, date__lte=end_date)
        else: # 'all' o sin selección
            entries_in_period = Entry.objects.all()

        unbalanced_entries_qs = entries_in_period.annotate(
            total_debit=Coalesce(Sum('transactions__debit'), Decimal('0.00'), output_field=DecimalField()),
            total_credit=Coalesce(Sum('transactions__credit'), Decimal('0.00'), output_field=DecimalField()),
            difference=F('total_debit') - F('total_credit')
        ).annotate(
            abs_difference=Abs('difference')
        ).filter(
            abs_difference__gte=Decimal('0.01') # Mismo filtro que en unbalanced_entries_view
        ).prefetch_related('transactions') # Prefetch para el bucle siguiente

        for entry in unbalanced_entries_qs:
            if min_date is None or entry.date < min_date:
                min_date = entry.date

            for t in entry.transactions.all(): # Usar las transacciones precargadas
                affected_accounts.add(t.account_id)

            # Buscar cuenta de contrapartida
            counterpart_account = None
            for keyword in AccountKeyword.objects.all():
                if keyword.keyword.lower() in entry.description.lower():
                    counterpart_account = keyword.account
                    break

            if counterpart_account:
                # Calcular sumas aquí, ya que no las tenemos de la agregación inicial
                current_debit = sum(t.debit for t in entry.transactions.all())
                current_credit = sum(t.credit for t in entry.transactions.all())

                affected_accounts.add(counterpart_account.id)
                Transaction.objects.create(
                    entry=entry,
                    account=counterpart_account,
                    debit=current_credit, # El débito de la contrapartida es el crédito actual
                    credit=current_debit  # El crédito de la contrapartida es el débito actual
                )
    else:
        # Si el formulario no es válido (poco probable aquí), redirigir sin periodo
        if form.is_bound:
            # El formulario tenía datos GET, pero el valor de 'period' no fue válido
            invalid_period = request.GET.get('period', '[Valor no encontrado]') # Debería existir si is_bound es True
            messages.error(request, f"Error al procesar el periodo seleccionado: '{invalid_period}' no es una opción válida.")
        else:
            # El formulario no tenía datos GET (ej. acceso directo a la URL sin parámetros)
            messages.error(request, "No se especificó un periodo para procesar. Por favor, use el filtro en la página anterior.")
        return redirect('reports:unbalanced_entries')

    for account_id in affected_accounts:
        if min_date: # Asegurarse de que min_date se estableció
            recalculate_balances_after_date.delay(
                min_date.strftime('%Y-%m-%d'), # Pasar fecha como string
                account_id
            )
    # Llamar a unbalanced_entries_view para obtener el contexto actualizado
    # y renderizar la plantilla directamente
    request.GET = request.GET.copy()  # Hacer el QueryDict mutable
    if period_value:
        request.GET['period'] = period_value # Asegurar que el periodo se pasa
    response = unbalanced_entries_view(request) # Llamar a la vista
    return response # Devolver la respuesta de la otra vista

def unbalanced_entries_view(request):
    add_breadcrumb(request, "Reporte de Descuadres", request.path)

    unbalanced_entries = None
    form = PeriodFilterForm(request.GET or None)
    start_date_filter = None
    end_date_filter = None

    if form.is_valid():
        start_date_filter, end_date_filter = form.get_date_range()

        if start_date_filter and end_date_filter:
            # Si get_date_range devolvió fechas (ej. 'Este Mes', 'Últimos 3 Años')
            entries_in_period = Entry.objects.filter(
                date__gte=start_date_filter,
                date__lte=end_date_filter
            )
        else:
            # Si get_date_range devolvió (None, None) (porque se eligió 'Todas las Fechas'
            # o es la carga inicial sin filtro), no se aplica filtro de fecha.
            entries_in_period = Entry.objects.all()

        unbalanced_entries = entries_in_period.annotate(
            total_debit=Coalesce(Sum('transactions__debit'), Decimal('0.00'), output_field=DecimalField()),
            total_credit=Coalesce(Sum('transactions__credit'), Decimal('0.00'), output_field=DecimalField()),
            difference=F('total_debit') - F('total_credit')
        ).annotate(
            abs_difference=Abs('difference')
        ).filter(abs_difference__gte=Decimal('0.01')).order_by('-date')

    context = {
        'form': form,
        'unbalanced_entries': unbalanced_entries,
    }
    return render(request, 'unbalanced_entries.html', context)




# Helper para obtener rango de fechas del form
def get_dates_from_form(form):
    start_date, end_date = form.get_date_range()
    return start_date, end_date


def merge_transactions_view(request):
    form = PeriodFilterForm(request.GET or None)
    entries_to_merge = []
    selected_period_params = request.GET.urlencode() # Para mantener el filtro al recargar

    if form.is_valid():
        start_date, end_date = get_dates_from_form(form)

        # 1. Encontrar IDs de asientos con transacciones duplicadas por cuenta
        duplicate_transactions = Transaction.objects.values(
            'entry_id', 'account_id'
        ).annotate(
            account_count=Count('id')
        ).filter(
            account_count__gt=1
        )

        entry_ids = duplicate_transactions.values_list('entry_id', flat=True)
        queryset = Entry.objects.filter(id__in=entry_ids).prefetch_related('transactions', 'transactions__account')

        if start_date:
            queryset = queryset.filter(date__gte=start_date)
        if end_date:
            queryset = queryset.filter(date__lte=end_date)

        for entry in queryset.order_by('-date', 'id'):
            transactions_by_account = {}
            has_duplicates_in_period = False # Verificar si la duplicidad está en el rango
            for t in entry.transactions.all():
                if t.account_id not in transactions_by_account:
                    transactions_by_account[t.account_id] = []
                transactions_by_account[t.account_id].append(t)

            duplicates_in_entry = []
            for account_id, transactions in transactions_by_account.items():
                if len(transactions) > 1:
                    has_duplicates_in_period = True
                    duplicates_in_entry.append({
                        'account': transactions[0].account,
                        'transactions': transactions,
                        'total_debit': sum(t.debit for t in transactions),
                        'total_credit': sum(t.credit for t in transactions),
                    })

            if has_duplicates_in_period: # Solo mostrar si hay duplicados reales en el periodo
                 entries_to_merge.append({
                     'entry': entry,
                     'duplicates': duplicates_in_entry
                 })

    context = {
        'form': form,
        'entries_to_merge': entries_to_merge,
        'selected_period_params': selected_period_params,
    }
    return render(request, 'merge_transactions.html', context)

@require_POST
@db_transaction.atomic
def process_merge_transactions(request):
    try:
        data = json.loads(request.body)
        # Espera una lista de objetos: [{'entry_id': X, 'account_id': Y, 'transaction_ids': [A, B, C]}, ...]
        transactions_to_merge_groups = data.get('groups', [])
        processed_count = 0
        affected_accounts_dates = {} # {account_id: min_date}

        if not transactions_to_merge_groups:
             return JsonResponse({'success': False, 'message': 'No se seleccionaron grupos para unificar.'})

        for group in transactions_to_merge_groups:
            entry_id = group.get('entry_id')
            account_id = group.get('account_id')
            transaction_ids = group.get('transaction_ids', [])

            if not entry_id or not account_id or not transaction_ids or len(transaction_ids) < 2:
                continue

            entry = Entry.objects.get(id=entry_id)
            account = Account.objects.get(id=account_id)

            transactions = Transaction.objects.filter(id__in=transaction_ids, entry_id=entry_id, account_id=account_id)

            if transactions.count() != len(transaction_ids):
                 messages.warning(request, f"No se pudieron unificar transacciones para la cuenta {account.name} en el asiento {entry_id}. Datos inconsistentes.")
                 continue

            totals = transactions.aggregate(
                total_debit=Sum('debit'),
                total_credit=Sum('credit')
            )

            Transaction.objects.create(
                entry=entry,
                account=account,
                debit=totals['total_debit'] or Decimal('0.00'),
                credit=totals['total_credit'] or Decimal('0.00')
            )

            deleted_count, _ = transactions.delete()
            processed_count += deleted_count

            if account_id not in affected_accounts_dates or entry.date < affected_accounts_dates[account_id]:
                affected_accounts_dates[account_id] = entry.date

        for acc_id, min_date in affected_accounts_dates.items():
             recalculate_balances_after_date.delay(min_date.strftime('%Y-%m-%d'), acc_id) # Asegurar formato fecha

        if processed_count > 0:
            message = f"Se unificaron {processed_count} transacciones correctamente."
            messages.success(request, message)
            return JsonResponse({'success': True, 'message': message})
        else:
             message = "No se procesó ninguna unificación (quizás los datos cambiaron)."
             messages.warning(request, message)
             return JsonResponse({'success': False, 'message': message})

    except Entry.DoesNotExist:
        return JsonResponse({'success': False, 'message': 'Error: Asiento no encontrado.'})
    except Account.DoesNotExist:
        return JsonResponse({'success': False, 'message': 'Error: Cuenta no encontrada.'})
    except json.JSONDecodeError:
         return JsonResponse({'success': False, 'message': 'Error en los datos recibidos.'})
    except Exception as e:
        logger.error(f"Error processing merge transactions: {e}", exc_info=True)
        return JsonResponse({'success': False, 'message': f'Ocurrió un error inesperado: {e}'})

# IDs de cuentas relevantes (ejemplos, ajustar según sea necesario)
# BRIDGE_ACCOUNT_ID para la cuenta puente usada en transferencias.

BRIDGE_ACCOUNT_ID = getattr(settings, 'BRIDGE_ACCOUNT_ID', 234) # Reemplaza None con el ID real si no está en settings

def detect_transfers_view(request):
    if not BRIDGE_ACCOUNT_ID:
        messages.error(request, "La cuenta puente no está configurada.")
        return render(request, 'detect_transfers.html', {'form': PeriodFilterForm(), 'error': True})

    form = PeriodFilterForm(request.GET or None)
    potential_transfers = []
    selected_period_params = request.GET.urlencode()

    if form.is_valid():
        start_date, end_date = get_dates_from_form(form)

        bridge_transactions = Transaction.objects.filter(
            account_id=BRIDGE_ACCOUNT_ID
        ).select_related('entry', 'account').order_by('entry__date', 'entry_id')

        if start_date:
            bridge_transactions = bridge_transactions.filter(entry__date__gte=start_date)
        if end_date:
            bridge_transactions = bridge_transactions.filter(entry__date__lte=end_date)

        # Agrupar por fecha y buscar pares (crédito/débito en cuenta puente por mismo importe)
        processed_entry_ids = set()
        candidates = {} # { (date, amount): {'credit': [tx1, tx2], 'debit': [tx3]} }

        for tx in bridge_transactions:
            if tx.entry_id in processed_entry_ids:
                continue

            entry_txs = list(tx.entry.transactions.select_related('account').all())

            # Asumimos transferencias simples: 2 transacciones por asiento
            # Una a/desde puente, la otra a/desde origen/destino
            if len(entry_txs) != 2: # Ignorar asientos complejos por ahora
                continue # Ignorar asientos complejos por ahora

            other_tx = None
            bridge_tx_in_entry = None
            for t in entry_txs:
                if t.account_id == BRIDGE_ACCOUNT_ID:
                    bridge_tx_in_entry = t
                else:
                    other_tx = t

            if not other_tx or not bridge_tx_in_entry:
                continue # No es el patrón esperado

            amount = bridge_tx_in_entry.debit if bridge_tx_in_entry.debit > 0 else bridge_tx_in_entry.credit
            tx_date = tx.entry.date
            key = (tx_date, amount)

            if key not in candidates:
                candidates[key] = {'credit_bridge': [], 'debit_bridge': []}

            if bridge_tx_in_entry.credit > 0: # La cuenta puente recibe dinero (origen -> puente)
                candidates[key]['credit_bridge'].append({
                    'entry': tx.entry,
                    'bridge_tx': bridge_tx_in_entry,
                    'origin_tx': other_tx, # Transacción de la cuenta origen (Débito)
                })
            elif bridge_tx_in_entry.debit > 0: # La cuenta puente entrega dinero (puente -> destino)
                 candidates[key]['debit_bridge'].append({
                    'entry': tx.entry,
                    'bridge_tx': bridge_tx_in_entry,
                    'other_tx': other_tx, # Transacción de la cuenta destino
                })
 
        for key, groups in candidates.items():
            for credit_info in groups['credit_bridge']:
                 # Evitar usar el mismo asiento dos veces si hubo error antes
                 if credit_info['entry'].id in processed_entry_ids: continue

                 for debit_info in groups['debit_bridge']:
                     if debit_info['entry'].id in processed_entry_ids: continue
                     # Evitar emparejar un asiento consigo mismo (improbable pero posible)
                     if credit_info['entry'].id == debit_info['entry'].id: continue

                     potential_transfers.append({
                         'date': key[0],
                         'amount': key[1],
                         'entry_from': credit_info['entry'],
                         'account_from': credit_info['origin_tx'].account,
                         'tx_from': credit_info['origin_tx'], # Débito origen
                         'entry_to': debit_info['entry'],
                         'account_to': debit_info['other_tx'].account,
                         'tx_to': debit_info['other_tx'], # Crédito destino
                     })
                     # Marcar como procesados para no volver a usarlos en otros pares
                     processed_entry_ids.add(credit_info['entry'].id)
                     processed_entry_ids.add(debit_info['entry'].id)
                     break # Pasar al siguiente crédito una vez encontrado un débito

    context = {
        'form': form,
        'potential_transfers': potential_transfers,
        'selected_period_params': selected_period_params,
        'error': not BRIDGE_ACCOUNT_ID
    }
    return render(request, 'detect_transfers.html', context)


@require_POST
@db_transaction.atomic
def process_simplify_transfers(request):
    if not BRIDGE_ACCOUNT_ID:
        return JsonResponse({'success': False, 'message': 'La cuenta puente no está configurada.'})

    try:
        data = json.loads(request.body)
        # Espera una lista de objetos: [{'entry_from_id': X, 'entry_to_id': Y, 'tx_from_id': A, 'tx_to_id': B}, ...]
        transfers_to_simplify = data.get('transfers', [])
        processed_count = 0
        affected_accounts_dates = {} # {account_id: min_date}

        if not transfers_to_simplify:
             return JsonResponse({'success': False, 'message': 'No se seleccionaron transferencias para simplificar.'})

        for transfer_info in transfers_to_simplify:
            entry_from_id = transfer_info.get('entry_from_id')
            entry_to_id = transfer_info.get('entry_to_id')

            if not entry_from_id or not entry_to_id:
                continue

            try:
                entry_from = Entry.objects.prefetch_related('transactions__account').get(id=entry_from_id)
                entry_to = Entry.objects.prefetch_related('transactions__account').get(id=entry_to_id)

                if entry_from.date != entry_to.date:
                    messages.warning(request, f"Las entradas {entry_from_id} y {entry_to_id} no tienen la misma fecha.")
                    continue

                tx_from_origin = None
                tx_to_bridge_in_from = None
                tx_from_bridge_in_to = None
                tx_to_destination = None
                amount = Decimal('0.00')

                for tx in entry_from.transactions.all():
                    if tx.account_id == BRIDGE_ACCOUNT_ID:
                        tx_to_bridge_in_from = tx
                    else:
                        tx_from_origin = tx
                for tx in entry_to.transactions.all():
                     if tx.account_id == BRIDGE_ACCOUNT_ID:
                         tx_from_bridge_in_to = tx
                     else:
                         tx_to_destination = tx

                if not all([tx_from_origin, tx_to_bridge_in_from, tx_from_bridge_in_to, tx_to_destination]):
                     messages.warning(request, f"No se encontró la estructura esperada en las entradas {entry_from_id}/{entry_to_id}.")
                     continue
                if tx_to_bridge_in_from.credit != tx_from_bridge_in_to.debit:
                     messages.warning(request, f"Los montos de la cuenta puente no coinciden en {entry_from_id}/{entry_to_id}.")
                     continue
                if tx_from_origin.debit != tx_to_destination.credit:
                     messages.warning(request, f"Los montos origen/destino no coinciden en {entry_from_id}/{entry_to_id}.")
                     continue

                amount = tx_from_origin.debit # o tx_to_destination.credit

                tx_to_bridge_in_from.delete()
                tx_from_bridge_in_to.delete()

                tx_to_destination.entry = entry_from
                tx_to_destination.save()

                entry_from.description = f"Transferencia simplificada: {tx_from_origin.account.name} -> {tx_to_destination.account.name} ({entry_from.description} / {entry_to.description})"
                entry_from.save()

                # Verificar que la entrada 'to' esté vacía antes de borrarla
                real_transaction_count = Transaction.objects.filter(entry=entry_to).count()
                logger.debug(f"Real transaction count for entry {entry_to.id} in DB before delete: {real_transaction_count}")

                if real_transaction_count == 0:
                    entry_to_date = entry_to.date # Guardar fecha antes de borrar
                    entry_to.delete()
                    processed_count += 1
                else:
                    # Esto no debería pasar con la lógica actual, pero es una salvaguarda
                    messages.error(request, f"Error: La entrada {entry_to.id} no quedó vacía (tiene {real_transaction_count} transacciones en BD) después de mover la transacción. No se eliminó.")
                    # Considerar revertir la operación o manejar el error de otra forma.
                    # Por ahora, se salta este par y no se cuenta como procesado.
                    continue

                # Registrar para recálculo (Origen, Destino y Puente)
                transfer_date = entry_from.date # o entry_to_date
                accounts_involved = [tx_from_origin.account_id, tx_to_destination.account_id, BRIDGE_ACCOUNT_ID]
                for acc_id in accounts_involved:
                     if acc_id not in affected_accounts_dates or transfer_date < affected_accounts_dates[acc_id]:
                         affected_accounts_dates[acc_id] = transfer_date

            except Entry.DoesNotExist:
                 messages.warning(request, f"No se encontró una de las entradas ({entry_from_id} o {entry_to_id}).")
                 continue
            except Exception as e_inner:
                 messages.error(request, f"Error procesando par {entry_from_id}/{entry_to_id}: {e_inner}")                 
                 continue

        for acc_id, min_date in affected_accounts_dates.items():
             recalculate_balances_after_date.delay(min_date.strftime('%Y-%m-%d'), acc_id)

        if processed_count > 0:
            message = f"Se simplificaron {processed_count} transferencias correctamente."
            messages.success(request, message)
            return JsonResponse({'success': True, 'message': message})
        else:
             message = "No se procesó ninguna simplificación."
             messages.warning(request, message)
             return JsonResponse({'success': False, 'message': message})

    except json.JSONDecodeError:
         return JsonResponse({'success': False, 'message': 'Error en los datos recibidos.'})
    except Exception as e:
        logger.error(f"Error processing simplify transfers: {e}", exc_info=True)
        return JsonResponse({'success': False, 'message': f'Ocurrió un error inesperado: {e}'})



def delete_empty_entries_view(request):
    form = PeriodFilterForm(request.GET or None)
    empty_entries = []
    selected_period_params = request.GET.urlencode()

    if form.is_valid():
        start_date, end_date = get_dates_from_form(form)

        queryset = Entry.objects.annotate(
            transaction_count=Count('transactions')
        ).filter(
            transaction_count=0
        )

        if start_date:
            queryset = queryset.filter(date__gte=start_date)
        if end_date:
            queryset = queryset.filter(date__lte=end_date)

        empty_entries = list(queryset.order_by('-date', 'id'))

    context = {
        'form': form,
        'empty_entries': empty_entries,
        'selected_period_params': selected_period_params,
    }
    return render(request, 'delete_empty_entries.html', context)

@require_POST
@db_transaction.atomic
def process_delete_empty_entries(request):
    try:
        data = json.loads(request.body)
        # Espera una lista de IDs: {'entry_ids': [1, 2, 3]}
        entry_ids_to_delete = data.get('entry_ids', [])

        if not entry_ids_to_delete:
             return JsonResponse({'success': False, 'message': 'No se seleccionaron asientos para eliminar.'})

        queryset = Entry.objects.annotate(
            transaction_count=Count('transactions')
        ).filter(
            id__in=entry_ids_to_delete,
            transaction_count=0
        )

        deleted_count, _ = queryset.delete()

        if deleted_count > 0:
            message = f"Se eliminaron {deleted_count} asientos vacíos correctamente."
            messages.success(request, message)
            return JsonResponse({'success': True, 'message': message})
        else:
             message = "No se eliminó ningún asiento (quizás ya no estaban vacíos o no existían)."
             messages.warning(request, message)
             return JsonResponse({'success': False, 'message': message})

    except json.JSONDecodeError:
         return JsonResponse({'success': False, 'message': 'Error en los datos recibidos.'})
    except Exception as e:
        logger.error(f"Error processing delete empty entries: {e}", exc_info=True)
        return JsonResponse({'success': False, 'message': f'Ocurrió un error inesperado: {e}'})

def get_looker_studio_data(task_id, start_date=None, end_date=None):
    """
    Fetches and formats data intended for Looker Studio (via Google Sheets).
    """
    transactions_qs = Transaction.objects.select_related('entry', 'account')

    if start_date and end_date:
        logger.debug(f"Task {task_id}: Filtering Looker data from {start_date} to {end_date}")
        transactions_qs = transactions_qs.filter(entry__date__gte=start_date, entry__date__lte=end_date)
    elif start_date: # Should not happen with PeriodFilterForm but good for robustness
        transactions_qs = transactions_qs.filter(entry__date__gte=start_date)
    elif end_date: # Should not happen with PeriodFilterForm but good for robustness
        transactions_qs = transactions_qs.filter(entry__date__lte=end_date)


    total_records = transactions_qs.count()
    processed_count = 0

    data_list = []
    for t in transactions_qs:
        data_list.append({
            'entry_id': t.entry.id,
            'date': t.entry.date,
            'description': t.entry.description,
            'transaction_id': t.id,
            'accountId': t.account.id,
            'account': t.account.get_full_hierarchy(),
            'debit': t.debit,
            'credit': t.credit,
            'parent_id': t.account.parent_id,
            'closed': t.account.closed,
        })
        processed_count += 1
        if processed_count % 500 == 0 or processed_count == total_records:
            cache.set(f'sheet_update_progress_{task_id}', {
                'processed': processed_count,
                'total': total_records,
                'status': 'processing',
                'stage': 'fetching'
            }, timeout=3600)
            logger.debug(f"Task {task_id}: Fetched {processed_count}/{total_records} records.")
    return data_list

def looker_data_viewer(request):
    add_breadcrumb(request, "Informe Looker Studio", request.path)
    form = PeriodFilterForm(request.GET or None) # Inicializar el formulario
    context = {
        'form': form, # Pasar el formulario a la plantilla
    }
    return render(request, 'looker_data_viewer.html', context)

@require_POST
def trigger_google_sheet_update(request):
    # El cliente (JavaScript) genera un task_id y lo envía en el header 'X-Task-ID'.
    # Esta vista utiliza ese task_id para actualizar el progreso en la caché,
    # y la vista de progreso lo utiliza para consultar dicho progreso.

    task_id = request.headers.get('X-Task-ID')
    if not task_id: # Fallback si no viene en header (no ideal para producción)
        task_id = uuid.uuid4().hex
        logger.warning(f"X-Task-ID header not found, generated task_id: {task_id}")


    logger.info(f"User {request.user} triggered Google Sheet update. Task ID: {task_id}")

    # Obtener el periodo del request.POST (enviado por JavaScript)
    period_value = request.POST.get('period')
    start_date, end_date = None, None

    if period_value: # Si se seleccionó un periodo (incluyendo la opción vacía que es válida)
        # Usar un diccionario para 'data' al inicializar el formulario programáticamente
        period_form = PeriodFilterForm(data={'period': period_value})
        if period_form.is_valid():
            start_date, end_date = period_form.get_date_range()
            logger.info(f"Task {task_id}: Applying period filter '{period_value}'. Date range: {start_date} to {end_date}")
        elif period_value:
            logger.warning(f"Task {task_id}: Invalid period value '{period_value}' received. Errors: {period_form.errors}")
            return JsonResponse({
                'status': 'error', 'message': f'Periodo inválido: {period_form.errors.as_text()}',
                'processed': 0, 'total': 0, 'stage': 'initialization_error'
            }, status=400)

    try:
        # Actualizar caché: Iniciando obtención de datos
        cache.set(f'sheet_update_progress_{task_id}', {
            'processed': 0,
            'total': 0, # Aún no sabemos el total de registros
            'status': 'processing',
            'stage': 'fetching_db_data',
            'message': 'Obteniendo datos de la base de datos...'
        }, timeout=3600)

        all_data_for_sheet_from_db = get_looker_studio_data(task_id, start_date=start_date, end_date=end_date)
        total_records = len(all_data_for_sheet_from_db)

        logger.info(f"Task {task_id}: Starting processing of {total_records} records.")
        
        # Preparar solo las filas de datos, la cabecera se manejará en google_sheets_updater.py
        data_rows_for_sheet = []
        for item in all_data_for_sheet_from_db:
            data_rows_for_sheet.append([
                item['entry_id'],
                item['date'].strftime('%Y-%m-%d') if item['date'] else '',
                item['description'],
                item['transaction_id'],
                item['accountId'],
                item['account'],
                str(item['debit']).replace('.', ',') if item['debit'] is not None else '0,00',
                str(item['credit']).replace('.', ',') if item['credit'] is not None else '0,00',
                item['parent_id'],
                item['closed'], # Convertir booleano a texto
            ])

        # Actualizar caché: Datos obtenidos, preparando para la hoja de cálculo
        cache.set(f'sheet_update_progress_{task_id}', {
            'processed': 0, # Aún no se ha subido nada a la hoja
            'total': total_records if total_records > 0 else 1,
            'status': 'processing',
            'stage': 'preparing_sheet_upload',
            'message': f'Se obtuvieron {total_records} registros. Actualizando Google Sheet...'
        }, timeout=3600)

        # Llamar a la función de actualización de la hoja, pasando fechas para borrado selectivo
        # update_google_sheet_with_data ahora maneja sus propias actualizaciones de caché y devuelve un dict
        update_result = update_google_sheet_with_data(data_rows_for_sheet, task_id, start_date, end_date)
        
        logger.info(f"Task {task_id}: Google Sheet update process finished. Result: {update_result.get('status')}, Message: {update_result.get('message')}")
        return JsonResponse(update_result)

    except Exception as e:
        logger.error(f"Task {task_id}: Error during Google Sheet update process: {e}", exc_info=True)
        current_progress = cache.get(f'sheet_update_progress_{task_id}', {})
        cache.set(f'sheet_update_progress_{task_id}', {
            'processed': current_progress.get('processed', 0), # Mantener el progreso que se tenía
            'total': current_progress.get('total', 1), # Mantener el total que se tenía, o 1 si no hay
            'status': 'error',
            'stage': current_progress.get('stage', 'error_view'), # Etapa donde ocurrió el error
            'message': f"Error durante el proceso de actualización de Google Sheet: {str(e)}"
        }, timeout=3600)
        return JsonResponse({'status': 'error', 'message': f"Error durante el proceso de actualización de Google Sheet: {e}"}, status=500)


def get_google_sheet_update_progress(request, task_id):
    progress_data = cache.get(f'sheet_update_progress_{task_id}')
    if progress_data:
        return JsonResponse(progress_data)
    else:
        return JsonResponse({
            'processed': 0, 
            'total': 1, # Evitar división por cero en el frontend si el total es 0
            'status': 'pending', 
            'message': 'Esperando inicio de la tarea o tarea no encontrada.'
            }, status=200)