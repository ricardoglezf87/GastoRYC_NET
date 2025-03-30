from urllib.parse import urlparse
from django.http import JsonResponse
from django.shortcuts import render, get_object_or_404, redirect
from django.forms import inlineformset_factory
from django.urls import reverse
from GARCA.utils import add_breadcrumb, remove_breadcrumb
from accounts.models import Account
from .models import Entry
from transactions.models import Transaction
from .forms import EntryForm
from transactions.forms import TransactionForm

def edit_entry(request, entry_id):
    entry = get_object_or_404(Entry, id=entry_id)
    accounts = Account.objects.all()
    accounts_with_hierarchy = sorted([
        (account.id, account.get_full_hierarchy()) for account in accounts
    ],key=lambda x: x[1]  
    )

    # Add breadcrumb
    add_breadcrumb(request, 'Editar entrada ' + str(entry_id), request.path)

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