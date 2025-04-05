from django.shortcuts import get_object_or_404
from django.http import JsonResponse
from django.views.decorators.http import require_POST
from django.views.decorators.csrf import csrf_exempt

from entries.models import Entry
from .models import Transaction
from .forms import TransactionForm
from async_tasks.tasks import recalculate_balances_after_date

@require_POST
@csrf_exempt
def update_transaction(request, transaction_id):
    transaction = get_object_or_404(Transaction, id=transaction_id)
    form = TransactionForm(request.POST, instance=transaction)
    if form.is_valid():

        entry = Entry.objects.get(id=transaction.entry_id)
        affected_accounts_before = set(
            t.account_id for t in entry.transactions.all()
        )

        transaction = form.save()

        affected_accounts_after = set(
            t.account_id for t in entry.transactions.all()
        )
        
        # Combinar todas las cuentas afectadas
        all_affected_accounts = affected_accounts_before.union(
            affected_accounts_after
        )

        for account_id in all_affected_accounts:
            recalculate_balances_after_date.delay(
                min(entry.date, entry.date),
                account_id
            )

        return JsonResponse({'success': True})
    else:
        return JsonResponse({'success': False, 'errors': form.errors})

@require_POST
@csrf_exempt
def add_transaction(request):
    form = TransactionForm(request.POST)
    if form.is_valid():
        transaction = form.save(commit=False)
        entry_id = request.POST.get('entry_id')
        if entry_id:
            transaction.entry_id = entry_id
        
        
        entry = Entry.objects.get(id=transaction.entry_id)
        affected_accounts_before = set(
            t.account_id for t in entry.transactions.all()
        )

        transaction = form.save()

        affected_accounts_after = set(
            t.account_id for t in entry.transactions.all()
        )
        
        # Combinar todas las cuentas afectadas
        all_affected_accounts = affected_accounts_before.union(
            affected_accounts_after
        )

        for account_id in all_affected_accounts:
            recalculate_balances_after_date.delay(
                min(entry.date, entry.date),
                account_id
            )

        return JsonResponse({'success': True, 'transaction_id': transaction.id})
    else:
        return JsonResponse({'success': False, 'errors': form.errors})

@require_POST
@csrf_exempt
def delete_transaction(request, transaction_id):
    if request.method == 'POST':
        transaction = get_object_or_404(Transaction, id=transaction_id)
        entry_id = transaction.entry_id

        entry = Entry.objects.get(id=transaction.entry_id)
        affected_accounts_before = set(
            t.account_id for t in entry.transactions.all()
        )

        transaction.delete()

        affected_accounts_after = set(
            t.account_id for t in entry.transactions.all()
        )
        
        # Combinar todas las cuentas afectadas
        all_affected_accounts = affected_accounts_before.union(
            affected_accounts_after
        )

        for account_id in all_affected_accounts:
            recalculate_balances_after_date.delay(
                min(entry.date, entry.date),
                account_id
            )
        
        return JsonResponse({'success': True})
    return JsonResponse({'success': False})