from django.shortcuts import render
from GARCA.utils import add_breadcrumb, clear_breadcrumbs
from entries.models import Entry

def unbalanced_entries_report(request):

    clear_breadcrumbs(request)
    add_breadcrumb(request, 'Entradas descuadradas' , request.path)

    unbalanced_entries = []
    entries = Entry.objects.all()

    for entry in entries:
        total_debit = sum(transaction.debit for transaction in entry.transactions.all())
        total_credit = sum(transaction.credit for transaction in entry.transactions.all())
        if total_debit != total_credit:
            unbalanced_entries.append(entry)

    return render(request, 'unbalanced_entries_report.html', {'unbalanced_entries': unbalanced_entries})
