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
    serializer_class = EntrySerializer # Reemplaza con tu serializer de asientos
    queryset = None # Se filtrará dinámicamente

    def get_serializer_context(self):
        """
        Añade el account_id del filtro al contexto del serializer.
        """
        context = super().get_serializer_context()
        try:
            # Obtener account_id de los parámetros de la query
            context['account_id'] = int(self.request.query_params.get('account_id'))
        except (TypeError, ValueError):
            context['account_id'] = None
        return context

    def get_queryset(self):
        # ... (tu lógica de filtrado existente para obtener el queryset de Entry) ...
        # Asegúrate que esta lógica filtre por account_id usando la relación
        # a través de Transaction, por ejemplo:
        # queryset = Entry.objects.filter(transactions__account_id=account_id, ...)
        # --------------------------------------------------------------------
        account_id = self.request.query_params.get('account_id')
        start_date_str = self.request.query_params.get('start_date')
        end_date_str = self.request.query_params.get('end_date')

        if not account_id or not start_date_str or not end_date_str:
            return Entry.objects.none()

        try:
            account_id = int(account_id)
            start_date = datetime.strptime(start_date_str, "%Y-%m-%d").date()
            end_date = datetime.strptime(end_date_str, "%Y-%m-%d").date()

            # Filtrar Entry basado en la transacción y la fecha
            queryset = Entry.objects.filter(
                transactions__account_id=account_id,
                date__gte=start_date,
                date__lte=end_date
            ).distinct() # distinct() es importante si un Entry pudiera coincidir múltiples veces

            print(f"Buscando asientos para cuenta {account_id} entre {start_date} y {end_date}")
            return queryset

        except (ValueError, TypeError) as e:
            print(f"Error en parámetros de búsqueda de asientos: {e}")
            return Entry.objects.none()
        except Exception as e:
             print(f"Error inesperado buscando asientos: {e}")
             traceback.print_exc()
             return Entry.objects.none()