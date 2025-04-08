from django.shortcuts import redirect, render
from GARCA.utils import add_breadcrumb, clear_breadcrumbs
from accounts.models import AccountKeyword
from entries.models import Entry
from datetime import datetime, timedelta
from dateutil.relativedelta import relativedelta

from transactions.models import Transaction


def recategorized_entries(request):
    
    period = request.GET.get('period', 'last_60_days')
    today = datetime.now().date()
    
    # Definir la fecha de inicio según el período
    date_filters = {
        'current_month': today.replace(day=1),
        'last_month': (today.replace(day=1) - timedelta(days=1)).replace(day=1),
        'last_30_days': today - timedelta(days=30),
        'last_60_days': today - timedelta(days=60),
        'last_180_days': today - timedelta(days=180),
        'current_year': today.replace(month=1, day=1),
        'last_year': (today - relativedelta(years=1)).replace(month=1, day=1),
        'year_before': (today - relativedelta(years=2)).replace(month=1, day=1),
        'last_3_years': (today - relativedelta(years=3)).replace(month=1, day=1),
        'last_5_years': (today - relativedelta(years=5)).replace(month=1, day=1),
        'last_10_years': (today - relativedelta(years=10)).replace(month=1, day=1),
        'all': None
    }

    if period not in date_filters:
        period = 'last_60_days'    

    start_date = date_filters.get(period)
    entries = Entry.objects.all()

    if start_date:
            entries_filtered = entries.filter(date__gte=start_date)


    unbalanced_entries = []
    
    for entry in entries_filtered:
        total_debit = sum(transaction.debit for transaction in entry.transactions.all())
        total_credit = sum(transaction.credit for transaction in entry.transactions.all())
        if total_debit != total_credit:
            unbalanced_entries.append(entry)

    for movement in unbalanced_entries:
            # Buscar cuenta de contrapartida
            counterpart_account = None
            for keyword in AccountKeyword.objects.all():
                if keyword.keyword.lower() in movement.description.lower():
                    counterpart_account = keyword.account
                    break                                 

            if counterpart_account:
                Transaction.objects.create(
                    entry=movement,
                    account=counterpart_account,
                    debit=sum(transaction.credit for transaction in entry.transactions.all()),
                    credit=sum(transaction.debit for transaction in entry.transactions.all())
                )


    return redirect('unbalanced_entries_report')

def unbalanced_entries_report(request):

    clear_breadcrumbs(request)
    add_breadcrumb(request, 'Entradas descuadradas' , request.path)

    period = request.GET.get('period', 'last_60_days')
    today = datetime.now().date()
    
    # Definir la fecha de inicio según el período
    date_filters = {
        'current_month': today.replace(day=1),
        'last_month': (today.replace(day=1) - timedelta(days=1)).replace(day=1),
        'last_30_days': today - timedelta(days=30),
        'last_60_days': today - timedelta(days=60),
        'last_180_days': today - timedelta(days=180),
        'current_year': today.replace(month=1, day=1),
        'last_year': (today - relativedelta(years=1)).replace(month=1, day=1),
        'year_before': (today - relativedelta(years=2)).replace(month=1, day=1),
        'last_3_years': (today - relativedelta(years=3)).replace(month=1, day=1),
        'last_5_years': (today - relativedelta(years=5)).replace(month=1, day=1),
        'last_10_years': (today - relativedelta(years=10)).replace(month=1, day=1),
        'all': None
    }

    if period not in date_filters:
        period = 'last_60_days'    

    start_date = date_filters.get(period)
    entries = Entry.objects.all()

    if start_date:
            entries_filtered = entries.filter(date__gte=start_date)

    unbalanced_entries = []
    
    for entry in entries_filtered:
        total_debit = sum(transaction.debit for transaction in entry.transactions.all())
        total_credit = sum(transaction.credit for transaction in entry.transactions.all())
        if total_debit != total_credit:
            unbalanced_entries.append(entry)

    return render(request, 'unbalanced_entries_report.html', {'selected_period':period,'unbalanced_entries': unbalanced_entries})
