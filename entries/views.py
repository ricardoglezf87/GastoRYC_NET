import traceback
from django.http import JsonResponse
from django.shortcuts import render, get_object_or_404, redirect
from django.forms import inlineformset_factory
from GARCA.utils import add_breadcrumb, get_breadcrumbs, go_back_breadcrumb, remove_breadcrumb
from accounts.models import Account
from async_tasks.tasks import recalculate_balances_after_date
from entries.serializers import EntrySerializer
from .models import Entry
from transactions.models import Transaction
from .forms import EntryForm
from transactions.forms import TransactionForm
from django.db import transaction
from datetime import datetime
from rest_framework import generics
from django.db.models import Q, Sum, F
from decimal import Decimal, InvalidOperation


def delete_entry(request, entry_id):
    
    entry = Entry.objects.get(id=entry_id)

    affected_accounts = set(
        t.account_id for t in entry.transactions.all()
    )

    for transaction in Transaction.objects.filter(entry_id=entry_id):
        transaction.delete()

    entry.delete()

    for account_id in affected_accounts:
        recalculate_balances_after_date.delay(
            min(entry.date, entry.date),
            account_id
        )

    go_back_breadcrumb(request)
    breadcrumbs = get_breadcrumbs(request)
    print(breadcrumbs)

    if breadcrumbs:
        # Redirige al usuario al último breadcrumb restante
        return redirect(breadcrumbs[-1][1])
    else:
        # Si no hay breadcrumbs, redirige a una página predeterminada
        return redirect('/accounts/account_tree/')

def edit_entry(request, entry_id):
    entry = get_object_or_404(Entry, id=entry_id)
    accounts = Account.objects.all()
    accounts_with_hierarchy = sorted([
        (account.id, account.get_full_hierarchy()) for account in accounts
    ],key=lambda x: x[1])

    add_breadcrumb(request, 'Editar entrada ' + str(entry_id), request.path)

    TransactionFormSet = inlineformset_factory(Entry, Transaction, form=TransactionForm, extra=1)
    if request.method == 'POST':
        form = EntryForm(request.POST, instance=entry)
        formset = TransactionFormSet(request.POST, instance=entry)
        if form.is_valid():
            with transaction.atomic():
                # Guardar la fecha original antes de los cambios
                original_date = entry.date
                
                # Obtener las cuentas afectadas antes y después del cambio
                affected_accounts_before = set(
                    t.account_id for t in entry.transactions.all()
                )
                
                form.save()
                formset.save()
                
                affected_accounts_after = set(
                    t.account_id for t in entry.transactions.all()
                )
                
                # Combinar todas las cuentas afectadas
                all_affected_accounts = affected_accounts_before.union(
                    affected_accounts_after
                )
                
                # Recalcular balances para todas las cuentas afectadas
                for account_id in all_affected_accounts:
                    recalculate_balances_after_date.delay(
                        min(original_date, entry.date),
                        account_id
                    )
                    
            return JsonResponse({'success': True})
        else:
            errors = {
                'form_errors': form.errors,
                'formset_errors': formset.errors,
                'non_form_errors': formset.non_form_errors()
            }
            return JsonResponse({'success': False, 'errors': errors})
    else:
        form = EntryForm(instance=entry)
        formset = TransactionFormSet(instance=entry)
    return render(request, 'edit_entry.html', {'form': form, 'formset': formset,
        'accounts_with_hierarchy': accounts_with_hierarchy, 'entry': entry})

def add_entry(request):

     # Add breadcrumb
    add_breadcrumb(request, 'Nueva entrada', request.path)

    if request.method == 'POST':
        form = EntryForm(request.POST)
        if form.is_valid():

            entry = form.save()

            Transaction.objects.create(
                entry=entry,
                account=Account.objects.get(id=1),
                debit=0,
                credit=0
            )
            entry.save()
            
            remove_breadcrumb(request, 'Nueva entrada' , request.path)
            return redirect('edit_entry', entry_id=entry.id)
    else:
        form = EntryForm()
    return render(request, 'add_entry.html', {'form': form})



class SearchEntries(generics.ListAPIView):
    """
    Busca asientos contables (Entry) que tengan transacciones
    asociadas a una cuenta específica, dentro de un rango de fechas
    y opcionalmente coincidiendo con un importe total.
    """
    serializer_class = EntrySerializer
    # El queryset se define dinámicamente en get_queryset

    def get_serializer_context(self):
        """
        Añade el account_id del filtro al contexto del serializer si es necesario.
        (Actualmente no parece usarse en EntrySerializer, pero es buena práctica).
        """
        context = super().get_serializer_context()
        try:
            context['account_id'] = int(self.request.query_params.get('account_id'))
        except (TypeError, ValueError, AttributeError):
            context['account_id'] = None
        return context

    def get_queryset(self):
        """
        Filtra los asientos (Entry) según los parámetros de la query:
        - account_id (obligatorio): ID de la cuenta en una de las transacciones.
        - start_date (obligatorio): Fecha de inicio (YYYY-MM-DD).
        - end_date (obligatorio): Fecha de fin (YYYY-MM-DD).
        - amount (opcional): Importe total del asiento (debe coincidir con la suma de débitos o créditos).
        """
        account_id_str = self.request.query_params.get('account_id')
        start_date_str = self.request.query_params.get('start_date')
        end_date_str = self.request.query_params.get('end_date')
        amount_str = self.request.query_params.get('amount') # Usar query_params consistentemente

        # Validaciones básicas de parámetros obligatorios
        if not account_id_str or not start_date_str or not end_date_str:
            print("SearchEntries: Faltan parámetros obligatorios (account_id, start_date, end_date).")
            return Entry.objects.none() # Devuelve queryset vacío si faltan

        try:
            # Conversión y validación de tipos
            account_id = int(account_id_str)
            start_date = datetime.strptime(start_date_str, "%Y-%m-%d").date()
            end_date = datetime.strptime(end_date_str, "%Y-%m-%d").date()
            amount_decimal = None
            if amount_str:
                try:
                    # Convertir a Decimal para comparación precisa
                    amount_decimal = Decimal(amount_str)
                except InvalidOperation:
                    print(f"SearchEntries: Importe inválido recibido: {amount_str}. Ignorando filtro de importe.")
                    # Podrías devolver un error 400 aquí si el importe es obligatorio o crucial

            # --- Filtrado Base por Cuenta y Fecha ---
            # Filtra Entries que tengan al menos una transacción con la cuenta dada
            # y cuya fecha esté en el rango especificado.
            queryset = Entry.objects.filter(
                transactions__account_id=account_id,
                date__gte=start_date,
                date__lte=end_date
            ).distinct() # distinct() es crucial si un Entry tiene múltiples transacciones con la misma cuenta

            print(f"SearchEntries: Filtro base -> Cuenta={account_id}, Rango={start_date} a {end_date}. Asientos encontrados: {queryset.count()}")

            # --- Filtrado Opcional por Importe ---
            if amount_decimal is not None:
                print(f"SearchEntries: Aplicando filtro de importe: {amount_decimal}")
                # Anotar la suma de débitos y créditos para cada Entry
                # Nota: Esto puede ser menos eficiente si tienes muchos asientos.
                # Considera almacenar total_debit/total_credit en el modelo Entry si el rendimiento es crítico.
                queryset = queryset.annotate(
                    total_debit=Sum('transactions__debit'),
                    total_credit=Sum('transactions__credit')
                )

                # Filtrar donde la suma de débitos O la suma de créditos coincida con el importe
                # Usamos una pequeña tolerancia por posibles problemas de redondeo decimal
                tolerance = Decimal('0.01')
                queryset = queryset.filter(
                    Q(total_debit__gte=amount_decimal - tolerance, total_debit__lte=amount_decimal + tolerance) |
                    Q(total_credit__gte=amount_decimal - tolerance, total_credit__lte=amount_decimal + tolerance)
                )
                # Alternativa (si solo buscas que *alguna* transacción en esa cuenta tenga ese importe, lo cual es diferente):
                # queryset = queryset.filter(transactions__account_id=account_id,
                #                            transactions__debit=amount_decimal) # O credit

                print(f"SearchEntries: Tras filtro de importe -> Asientos encontrados: {queryset.count()}")

            # Ordenar resultados (opcional, por fecha por ejemplo)
            queryset = queryset.order_by('date', 'id')

            return queryset

        except (ValueError, TypeError) as e:
            # Error en conversión de parámetros (ID no numérico, fecha inválida)
            print(f"SearchEntries: Error en parámetros de búsqueda: {e}")
            return Entry.objects.none()
        except Account.DoesNotExist:
             print(f"SearchEntries: Cuenta con ID {account_id_str} no encontrada.")
             return Entry.objects.none()
        except Exception as e:
             # Captura cualquier otro error inesperado
             print(f"SearchEntries: Error inesperado buscando asientos: {e}")
             traceback.print_exc()
             return Entry.objects.none()