from urllib.parse import urlparse
from django.http import JsonResponse
from django.shortcuts import render, get_object_or_404, redirect
from django.forms import inlineformset_factory
from django.urls import reverse
from GARCA.utils import add_breadcrumb, remove_breadcrumb
from accounts.models import Account
from async_tasks.tasks import recalculate_balances_after_date
from .models import Entry
from transactions.models import Transaction
from .forms import EntryForm
from transactions.forms import TransactionForm
from django.db import transaction

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
        if form.is_valid() and formset.is_valid():
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
            remove_breadcrumb(request, 'Nueva entrada' , request.path)
            return redirect('edit_entry', entry_id=entry.id)
    else:
        form = EntryForm()
    return render(request, 'add_entry.html', {'form': form})