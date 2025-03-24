from django.shortcuts import render, get_object_or_404, redirect
from GARCA.utils import add_breadcrumb, clear_breadcrumbs, remove_breadcrumb
from .models import Account
from .forms import AccountForm

def account_tree_view(request):

    clear_breadcrumbs(request)

    accounts = Account.objects.filter(parent=None).prefetch_related('children')
    return render(request, 'account_tree.html', {'accounts': accounts})

def edit_account(request, account_id):
    account = get_object_or_404(Account, id=account_id)
    parents = Account.objects.exclude(pk=account_id)
    transactions = account.transaction_set.all().order_by('-entry__date', '-id')

    balance = 0
    for transaction in reversed(transactions):
        balance += transaction.debit - transaction.credit
        transaction.balance = balance

    # Add breadcrumb
    add_breadcrumb(request, 'Editar cuenta ' + str(account_id), request.path)

    if request.method == 'POST':
        form = AccountForm(request.POST, instance=account)
        if form.is_valid():
            form.save()
            return redirect('account_tree')
    else:
        form = AccountForm(instance=account)
              
    return render(request, 'edit_account.html', {'form': form, 'account': account, 'parents': parents, 'transactions': transactions})

def add_account(request):

    # Add breadcrumb
    add_breadcrumb(request, 'Nueva cuenta' , request.path)

    if request.method == 'POST':
        form = AccountForm(request.POST)
        if form.is_valid():
            account = form.save()
            remove_breadcrumb(request, 'Nueva cuenta' , request.path)
            return redirect('edit_account', account_id=account.id)
    else:
        form = AccountForm()
    return render(request, 'add_account.html', {'form': form})