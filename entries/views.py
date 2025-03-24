from django.http import JsonResponse
from django.shortcuts import render, get_object_or_404, redirect
from django.forms import inlineformset_factory
from django.urls import reverse
from .models import Entry
from transactions.models import Transaction
from .forms import EntryForm
from transactions.forms import TransactionForm

def edit_entry(request, entry_id):
    entry = get_object_or_404(Entry, id=entry_id)
        
    back = request.GET.get('back')
    if back and back.isdigit():  
        back = int(back)

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
    return render(request, 'edit_entry.html', {'back': back,'form': form, 'formset': formset, 'entry': entry})

def add_entry(request):
    back = request.GET.get('back')
    if back and back.isdigit():  
        back = int(back)

    if request.method == 'POST':
        form = EntryForm(request.POST)
        if form.is_valid():
            entry = form.save()
            if back is not None:
                return redirect(reverse('edit_entry', kwargs={'entry_id': entry.id}) + '?back=' + str(back))
            else:
                return redirect('edit_entry', entry_id=entry.id)
    else:
        form = EntryForm()
    return render(request, 'add_entry.html', {'back': back,'form': form})