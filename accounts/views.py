from django.shortcuts import render, get_object_or_404, redirect
from django.http import JsonResponse
from django.views.decorators.csrf import csrf_exempt
from django.db.models import Sum, F, Q  # Añadir esta línea
from GARCA.utils import add_breadcrumb, clear_breadcrumbs, remove_breadcrumb
from .models import Account, AccountKeyword
from .forms import AccountForm
import json
from django.core.paginator import Paginator

def account_tree_view(request):

    clear_breadcrumbs(request)

    accounts = Account.objects.filter(parent=None).prefetch_related('children')
    return render(request, 'account_tree.html', {'accounts': accounts})

def edit_account(request, account_id):
    account = get_object_or_404(Account, id=account_id)
    parents = Account.objects.exclude(pk=account_id)
    parents_with_hierarchy = sorted([
        (parent.id, parent.get_full_hierarchy()) for parent in parents
    ],key=lambda x: x[1])        

    # Add breadcrumb
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
    ],key=lambda x: x[1])

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
    return render(request, 'add_account.html', {'form': form, 'parents': parents_with_hierarchy,})

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
    account = get_object_or_404(Account, id=account_id)
    
    # Parámetros de paginación y búsqueda
    page = int(request.GET.get('page', 1))
    size = int(request.GET.get('size', 25))
    search = request.GET.get('search', '')
    
    # Query base con optimizaciones - Ordenar del más viejo al más nuevo
    transactions = account.transaction_set.all()\
        .select_related('entry')\
        .prefetch_related('entry__transactions', 'entry__attachments')\
        .order_by('-entry__date', '-id')

    # Aplicar filtro de búsqueda si existe
    if search:
        transactions = transactions.filter(
            Q(entry__description__icontains=search) |
            Q(entry__transactions__account__name__icontains=search)
        ).distinct()
    
    total_count = transactions.count()
    
    # Calcular balance inicial sumando todas las transacciones anteriores a la página actual
    paginator = Paginator(transactions, size)
    page_obj = paginator.get_page(page)
    
    # Si no hay transacciones, devolver array vacío
    if not page_obj:
        return JsonResponse({
            "data": [],
            "last_page": paginator.num_pages,
            "total": total_count
        })
    
    # Calcular balance acumulado hasta el inicio de la página actual
    balance = 0
    
    previous_transactions = transactions.filter(
        entry__date__lt=page_obj[0].entry.date
    ) | transactions.filter(
        entry__date=page_obj[0].entry.date,
        id__lt=page_obj[0].id
    )
    print(previous_transactions.query)
    balance = previous_transactions.aggregate(
        balance=Sum(F('debit') - F('credit'))
    )['balance'] or 0
    
    data = []
    for transaction in page_obj:
        filtered_transactions = [
            t for t in transaction.entry.transactions.all() 
            if t.account_id != account.id
        ]
        
        # Usar el balance precalculado
        balance = transaction.entry.balance.get(str(account_id), 0)
        
        data.insert(0,{
            "date": transaction.entry.date.strftime('%Y-%m-%d'),
            "date_link": f"/entries/edit_entry/{transaction.entry.id}/",
            "description": transaction.entry.description,
            "counterparts": "\n".join([t.account.get_full_hierarchy() for t in filtered_transactions]),
            "debit": float(transaction.debit),
            "credit": float(transaction.credit),
            "balance": balance,
            "has_attachments": bool(transaction.entry.attachments.exists()),
            "entry_id": transaction.entry.id
        })

    data.reverse()
    
    return JsonResponse({
        "data": data,
        "last_page": paginator.num_pages,
        "total": total_count
    })