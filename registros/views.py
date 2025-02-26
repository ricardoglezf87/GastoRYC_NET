from django.shortcuts import render, get_object_or_404
from .models import Entry, Account
from .forms import AccountForm

def entry_detail_view(request, entry_id):
    entry = get_object_or_404(Entry, id=entry_id)
    return render(request, 'admin/entry_detail.html', {'entry': entry})

def account_tree_view(request):
    accounts = Account.objects.filter(parent=None).prefetch_related('children')
    return render(request, 'admin/account_tree.html', {'accounts': accounts})

def edit_account(request, account_id):
    account = get_object_or_404(Account, id=account_id)
    if request.method == 'POST':
        form = AccountForm(request.POST, instance=account)
        if form.is_valid():
            form.save()
            # Redirigir a la página de detalle de la cuenta o a otra página
    else:
        form = AccountForm(instance=account)
    return render(request, 'admin/edit_account.html', {'form': form, 'account': account})