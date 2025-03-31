from django.shortcuts import get_object_or_404
from django.http import JsonResponse
from django.views.decorators.http import require_POST
from django.views.decorators.csrf import csrf_exempt
from .models import Transaction
from .forms import TransactionForm

@require_POST
@csrf_exempt
def update_transaction(request, transaction_id):
    transaction = get_object_or_404(Transaction, id=transaction_id)
    form = TransactionForm(request.POST, instance=transaction)
    if form.is_valid():
        form.save()
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
        transaction.save()
        return JsonResponse({'success': True, 'transaction_id': transaction.id})
    else:
        return JsonResponse({'success': False, 'errors': form.errors})

def delete_transaction(request, transaction_id):
    if request.method == 'POST':
        transaction = get_object_or_404(Transaction, id=transaction_id)
        transaction.delete()
        return JsonResponse({'success': True})
    return JsonResponse({'success': False})