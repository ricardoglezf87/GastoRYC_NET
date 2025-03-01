from django.shortcuts import render, get_object_or_404, redirect
from django.http import JsonResponse
from django.views.decorators.csrf import csrf_exempt
from .models import Entry, Account, Attachment, Transaction
from .forms import AccountForm, EntryForm, TransactionForm
from django.contrib.contenttypes.models import ContentType
from django.forms import inlineformset_factory
from django.views.decorators.http import require_POST
import json

def account_tree_view(request):
    accounts = Account.objects.filter(parent=None).prefetch_related('children')
    return render(request, 'admin/account_tree.html', {'accounts': accounts})

def edit_account(request, account_id):
    account = get_object_or_404(Account, id=account_id)
    parents = Account.objects.exclude(pk=account_id) 
    transactions = account.transaction_set.all().order_by('-entry__date', '-id') 

    balance = 0
    for transaction in reversed(transactions):
        balance += transaction.debit - transaction.credit
        transaction.balance = balance

    if request.method == 'POST':
        form = AccountForm(request.POST, instance=account)
        if form.is_valid():
            form.save()
            return redirect('account_tree')
    else:
        form = AccountForm(instance=account)
              
    return render(request, 'admin/edit_account.html', {'form': form, 'account': account, 'parents': parents, 'transactions': transactions})

def add_account(request):
    if request.method == 'POST':
        form = AccountForm(request.POST)
        if form.is_valid():
            account = form.save()
            return redirect('edit_account', account_id=account.id)
    else:
        form = AccountForm()
    return render(request, 'admin/add_account.html', {'form': form})

@csrf_exempt
def upload_attachments(request, account_id):
    if request.method == 'POST':
        account = get_object_or_404(Account, id=account_id)
        for file in request.FILES.getlist('files'):
            Attachment.objects.create(
                file=file,
                content_type=ContentType.objects.get_for_model(Account),
                description=file.name,
                object_id=account.id
            )
        return JsonResponse({'success': True})
    return JsonResponse({'success': False})

def edit_entry(request, entry_id):
    entry = get_object_or_404(Entry, id=entry_id)
    TransactionFormSet = inlineformset_factory(Entry, Transaction, form=TransactionForm, extra=1)
    if request.method == 'POST':
        form = EntryForm(request.POST, instance=entry)
        formset = TransactionFormSet(request.POST, instance=entry)
        if form.is_valid():
            form.save()
            return JsonResponse({'success': True})
        else:
            errors = {
                'form_errors': form.errors,
                'formset_errors': formset.errors,
                'non_form_errors': formset.non_form_errors()
            }
            return JsonResponse({'success': False, 'errors': form.errors})
    else:
        form = EntryForm(instance=entry)
        formset = TransactionFormSet(instance=entry)
    return render(request, 'admin/edit_entry.html', {'form': form, 'formset': formset, 'entry': entry})

def add_entry(request):
    if request.method == 'POST':
        form = EntryForm(request.POST)
        if form.is_valid():
            entry = form.save()
            return redirect('edit_entry', entry_id=entry.id)
    else:
        form = EntryForm()
    return render(request, 'admin/add_entry.html', {'form': form})

@csrf_exempt
def upload_entry_attachments(request, entry_id):
    if request.method == 'POST':
        entry = get_object_or_404(Entry, id=entry_id)
        for file in request.FILES.getlist('files'):
            Attachment.objects.create(
                file=file,
                content_type=ContentType.objects.get_for_model(Entry),
                description=file.name,
                object_id=entry.id
            )
        return JsonResponse({'success': True})
    return JsonResponse({'success': False})

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

@require_POST
@csrf_exempt
def update_attachment_description(request, attachment_id):
    attachment = get_object_or_404(Attachment, id=attachment_id)
    data = json.loads(request.body)
    attachment.description = data.get('description', '')
    attachment.save()
    return JsonResponse({'success': True})

@require_POST
@csrf_exempt
def delete_attachment(request, attachment_id):
    attachment = get_object_or_404(Attachment, id=attachment_id)
    attachment.delete()
    return JsonResponse({'success': True})