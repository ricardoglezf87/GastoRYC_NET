from django.shortcuts import render, get_object_or_404, redirect
from django.http import JsonResponse
from django.urls import reverse
from django.views.decorators.csrf import csrf_exempt
from .models import Entry, Account, Attachment, Transaction
from .forms import AccountForm, EntryForm, BankImportForm, TransactionForm
from django.contrib.contenttypes.models import ContentType
from django.forms import inlineformset_factory
from django.views.decorators.http import require_POST
import json
import io
import csv
from decimal import Decimal
from datetime import datetime
from django.contrib import messages
from django.db import transaction
from django.views import View
from django.utils.decorators import method_decorator

class BankImportView(View):
    template_name = 'import_movements.html'
    
    def get(self, request):
        form = BankImportForm()
        return render(request, self.template_name, {'form': form})
    
    def post(self, request):
        form = BankImportForm(request.POST, request.FILES)
        if form.is_valid():
            bank_provider = form.cleaned_data['bank_provider']
            account = form.cleaned_data['account']
            file = request.FILES['file']
            
            # Leer contenido del archivo
            if file.name.endswith('.csv'):
                # Procesar el archivo según el proveedor bancario
                if bank_provider == 'ing':
                    preview_data, import_data = self.process_ing_file(file)
                    
                    # Si es importar, procesar los datos
                    if 'import' in request.POST and import_data:
                        success_count = self.import_movements(account, import_data)
                        messages.success(request, f'Se han importado {success_count} movimientos correctamente.')
                        return redirect('bank_import')
            else:
                messages.error(request, 'El archivo debe ser CSV')
                
        return render(request, self.template_name, {'form': form})
    
    def process_ing_file(self, file):
        """Procesa un archivo CSV de ING Direct"""
        # Decodificar el archivo
        content = file.read().decode('latin-1')
        csv_data = []
        import_data = []
        
        # Procesar con CSV
        csv_reader = csv.reader(io.StringIO(content), delimiter=';')
        
        # Saltamos las primeras 5 líneas (encabezados)
        for _ in range(5):
            next(csv_reader, None)
        
        # Procesamos las líneas de datos
        for row in csv_reader:
            if len(row) >= 6:  # Asegurar que la fila tiene suficientes columnas
                try:
                    # Extraer datos relevantes
                    date_str = row[0]
                    category = row[1]
                    subcategory = row[2]
                    description = row[3]
                    comment = row[4]
                    amount_str = row[5].replace('.', '').replace(',', '.')
                    
                    # Validar fecha
                    if date_str:
                        date = datetime.strptime(date_str, '%d/%m/%Y').date()
                        
                        # Validar importe
                        if amount_str:
                            try:
                                amount = Decimal(amount_str)
                                
                                # Preparar datos para vista previa
                                csv_data.append({
                                    'date': date,
                                    'category': f"{category} - {subcategory}",
                                    'description': description,
                                    'amount': amount
                                })
                                
                                # Preparar datos para importación
                                import_data.append({
                                    'date': date,
                                    'category': category,
                                    'subcategory': subcategory,
                                    'description': description,
                                    'comment': comment,
                                    'amount': amount
                                })
                            except:
                                # Ignorar líneas con importes inválidos
                                pass
                except:
                    # Ignorar líneas con formato incorrecto
                    pass
                    
        return csv_data, import_data
    
    @transaction.atomic
    def import_movements(self, account, import_data):
        """Importa los movimientos a la base de datos"""
        count = 0
        
        for movement in import_data:
            # Crear una entrada por cada movimiento
            entry = Entry.objects.create(
                date=movement['date'],
                description=f"{movement['description']}"
            )
            
            # Determinar si es débito o crédito
            amount = movement['amount']
            debit = 0
            credit = 0
            
            if amount > 0:
                debit = amount
            else:
                credit = abs(amount)
            
            # Crear la transacción
            Transaction.objects.create(
                entry=entry,
                account=account,
                debit=debit,
                credit=credit
            )
            
            count += 1
        
        return count

@method_decorator(csrf_exempt, name='dispatch')
class BankImportPreviewView(View):
    def post(self, request):
        form = BankImportForm(request.POST, request.FILES)
        if form.is_valid():
            bank_provider = form.cleaned_data['bank_provider']
            file = request.FILES['file']
            
            if file.name.endswith('.csv'):
                if bank_provider == 'ing':
                    preview_data, _ = BankImportView().process_ing_file(file)
                    return JsonResponse({'success': True, 'preview_data': preview_data})
        return JsonResponse({'success': False})

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
    return render(request, 'admin/edit_entry.html', {'back': back,'form': form, 'formset': formset, 'entry': entry})

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
    return render(request, 'admin/add_entry.html', {'back': back,'form': form})

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