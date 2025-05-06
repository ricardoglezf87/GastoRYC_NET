from django.shortcuts import render, get_object_or_404, redirect
from django.http import JsonResponse
from django.views.decorators.csrf import csrf_exempt
from GARCA.utils import add_breadcrumb, clear_breadcrumbs, remove_breadcrumb
from accounts.serializers import AccountSerializer
from .models import Account, AccountKeyword
from .forms import AccountForm
import json
from datetime import datetime, timedelta
from dateutil.relativedelta import relativedelta
from rest_framework import generics

def account_tree_view(request):
    clear_breadcrumbs(request)
    show_closed_flag = request.GET.get('show_closed') == 'on'
    accounts = Account.objects.filter(parent=None).prefetch_related('children')
    return render(request, 'account_tree.html', {'accounts': accounts, 'show_closed': show_closed_flag})

def edit_account(request, account_id):
    account = get_object_or_404(Account, id=account_id)
    parents = Account.objects.exclude(pk=account_id)
    parents_with_hierarchy = sorted([
        (parent.id, parent.get_full_hierarchy()) for parent in parents
    ], key=lambda x: x[1])

    add_breadcrumb(request, 'Editar cuenta ' + str(account_id), request.path)

    if request.method == 'POST':
        form = AccountForm(request.POST, instance=account)
        if form.is_valid():
            form.save()
            return redirect('account_tree')
    else:
        form = AccountForm(instance=account)

    return render(request, 'edit_account.html', {'form': form, 'account': account, 'parents': parents_with_hierarchy})

def add_account(request):
    parents = Account.objects.all()
    parents_with_hierarchy = sorted([
        (parent.id, parent.get_full_hierarchy()) for parent in parents
    ], key=lambda x: x[1])

    add_breadcrumb(request, 'Nueva cuenta', request.path)

    if request.method == 'POST':
        form = AccountForm(request.POST)
        if form.is_valid():
            account = form.save()
            remove_breadcrumb(request, 'Nueva cuenta', request.path)
            return redirect('edit_account', account_id=account.id)
    else:
        form = AccountForm()
    return render(request, 'add_account.html', {'form': form, 'parents': parents_with_hierarchy})

@csrf_exempt
def add_keyword(request, account_id):
    if request.method == 'POST':
        account = get_object_or_404(Account, id=account_id)
        data = json.loads(request.body)
        keyword = AccountKeyword.objects.create(account=account, keyword=data['keyword'])
        return JsonResponse({'success': True, 'keyword_id': keyword.id})
    return JsonResponse({'success': False})

@csrf_exempt
def update_keyword(request, keyword_id):
    if request.method == 'POST':
        keyword = get_object_or_404(AccountKeyword, id=keyword_id)
        data = json.loads(request.body)
        keyword.keyword = data['keyword']
        keyword.save()
        return JsonResponse({'success': True})
    return JsonResponse({'success': False})

@csrf_exempt
def delete_keyword(request, keyword_id):
    if request.method == 'POST':
        keyword = get_object_or_404(AccountKeyword, id=keyword_id)
        keyword.delete()
        return JsonResponse({'success': True})
    return JsonResponse({'success': False})

def get_account_transactions(request, account_id):
    try:
        account = get_object_or_404(Account, id=account_id)
        period = request.GET.get('period', 'last_60_days')
        today = datetime.now().date()

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

        transactions = account.transaction_set.all()\
            .select_related('entry')\
            .prefetch_related('entry__transactions', 'entry__attachments')\
            .order_by('-entry__date', '-id')

        start_date = date_filters.get(period)
        if start_date:
            transactions = transactions.filter(entry__date__gte=start_date)

        filters_json = request.GET.get('filters', '{}')
        try:
            filters = json.loads(filters_json)
            if filters.get('description'):
                transactions = transactions.filter(
                    entry__description__icontains=filters['description']
                )
            if filters.get('counterparts'):
                transactions = transactions.filter(
                    entry__transactions__account__name__icontains=filters['counterparts']
                )
        except json.JSONDecodeError:
            pass

        transactions = transactions.distinct()

        data = []
        for transaction in transactions:
            filtered_transactions = [
                t for t in transaction.entry.transactions.all()
                if t.account_id != account.id
            ]

            data.append({
                "date": transaction.entry.date.strftime('%Y-%m-%d'),
                "date_link": f"/entries/edit_entry/{transaction.entry.id}/",
                "description": transaction.entry.description,
                "counterparts": "\n".join([t.account.get_full_hierarchy() for t in filtered_transactions]),
                "debit": float(transaction.debit),
                "credit": float(transaction.credit),
                "balance": transaction.entry.balance.get(str(account_id), 0),
                "has_attachments": bool(transaction.entry.attachments.exists()),
                "entry_id": transaction.entry.id
            })

        return JsonResponse({
            'data': data,
            'total': len(data)
        })

    except Exception as e:
        print(f"Error: {str(e)}")
        return JsonResponse({
            'data': [],
            'total': 0,
            'error': str(e)
        })

class AccountListView(generics.ListAPIView):
    queryset = Account.objects.all()
    serializer_class = AccountSerializer