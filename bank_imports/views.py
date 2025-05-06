from django.shortcuts import render, redirect, get_object_or_404
from django.http import JsonResponse
from django.views.decorators.csrf import csrf_exempt
from GARCA.utils import add_breadcrumb, clear_breadcrumbs
from accounts.models import Account, AccountKeyword
from async_tasks.tasks import recalculate_balances_after_date
from entries.models import Entry
from transactions.models import Transaction
from .forms import  BankImportForm
import io
import csv
from decimal import Decimal
from datetime import datetime
from django.contrib import messages
from django.db import transaction
from django.views import View
from django.utils.decorators import method_decorator


class ImportMovementsMixin:
    @transaction.atomic
    def import_movements(self, account, import_data):
        count = 0
        affected_accounts = set()
        min_date = None
        affected_accounts.add(account.id)

        for movement in import_data:
            # Buscar cuenta de contrapartida
            counterpart_account = None
            for keyword in AccountKeyword.objects.all():
                if keyword.keyword.lower() in movement['description'].lower():
                    counterpart_account = keyword.account
                    break

            # Crear una entrada por cada movimiento
            entry = Entry.objects.create(
                date=movement['date'],
                description=movement['description']
            )

            # Determinar si es débito o crédito
            amount = movement['amount']
            debit = amount if amount > 0 else 0
            credit = abs(amount) if amount < 0 else 0

            # Crear transacciones
            Transaction.objects.create(
                entry=entry,
                account=account,
                debit=debit,
                credit=credit
            )

            if counterpart_account:
                affected_accounts.add(counterpart_account.id)
                Transaction.objects.create(
                    entry=entry,
                    account=counterpart_account,
                    debit=credit,
                    credit=debit
                )

            if min_date is None or movement['date'] < min_date:
                min_date = movement['date']

            count += 1

        for account_id in affected_accounts:
            recalculate_balances_after_date.delay(
                min_date,
                account_id
            )

        return count

class BankImportView(ImportMovementsMixin, View):
    template_name = 'import_movements.html'

    def get(self, request):
        clear_breadcrumbs(request)
        add_breadcrumb(request, 'Importar movimientos' , request.path)
        form = BankImportForm()
        return render(request, self.template_name, {'form': form})

    def post(self, request):
        form = BankImportForm(request.POST, request.FILES)
        if form.is_valid():
            bank_provider = form.cleaned_data['bank_provider']
            account = form.cleaned_data['account']
            file = request.FILES['file']
            
            if file.name.endswith('.csv'):
                if bank_provider == 'ing':
                    preview_data, import_data = self.process_ing_file(file)
                    
                    if 'import' in request.POST and import_data:
                        # Separar movimientos duplicados y no duplicados
                        duplicates = []
                        non_duplicates = []
                        
                        for movement in import_data:
                            if self.is_duplicate(account, movement):
                                duplicates.append(movement)
                            else:
                                non_duplicates.append(movement)
                        
                        # Importar primero los no duplicados
                        if non_duplicates:
                            success_count = self.import_movements(account, non_duplicates)
                            if duplicates:
                                messages.success(request, f'Se han importado {success_count} movimientos, pero se han encontrado algunos movimientos duplicados')
                            else:
                                messages.success(request, f'Se han importado {success_count} movimientos')
                        
                        # Si hay duplicados, mostrar página de confirmación
                        if duplicates:
                            # Convertir fechas y decimales a string antes de guardar en sesión
                            session_duplicates = []
                            for movement in duplicates:
                                movement_copy = movement.copy()
                                movement_copy['date'] = movement['date'].strftime('%Y-%m-%d')
                                movement_copy['amount'] = str(movement['amount'])
                                session_duplicates.append(movement_copy)
                            
                            request.session['pending_import'] = {
                                'account_id': account.id,
                                'duplicates': session_duplicates
                            }
                            return redirect('bank_import_duplicates')
                        
                        return redirect('bank_import')
            else:
                messages.error(request, 'El archivo debe ser CSV')
                
        return render(request, self.template_name, {'form': form})

    def is_duplicate(self, account, movement):
        """Helper method to check if a movement is duplicate"""
        return Transaction.objects.filter(
            account=account,
            entry__date=movement['date'],
            debit=movement['amount'] if movement['amount'] > 0 else 0,
            credit=abs(movement['amount']) if movement['amount'] < 0 else 0
        ).exists()

    def process_ing_file(self, file):
        """Procesa un archivo CSV de ING Direct"""
        # Decodificar el archivo
        content = file.read().decode('utf-8')
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
    
    def find_duplicates(self, account, import_data):
        duplicates = []
        for movement in import_data:
            # Buscar movimientos existentes con la misma fecha y cantidad
            existing = Transaction.objects.filter(
                account=account,
                entry__date=movement['date'],
                debit=movement['amount'] if movement['amount'] > 0 else 0,
                credit=abs(movement['amount']) if movement['amount'] < 0 else 0
            ).exists()
            
            if existing:
                duplicates.append(movement)
        return duplicates


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


class BankImportDuplicatesView(ImportMovementsMixin, View):
    template_name = 'import_duplicates.html'

    def get(self, request):
        pending_import = request.session.get('pending_import')
        if not pending_import:
            return redirect('bank_import')

        # Convertir fechas de string a date y amounts de string a Decimal
        duplicates = pending_import['duplicates']
        for movement in duplicates:
            movement['date'] = datetime.strptime(movement['date'], '%Y-%m-%d').date()
            movement['amount'] = Decimal(movement['amount'])

        return render(request, self.template_name, {
            'duplicates': duplicates
        })

    def post(self, request):
        pending_import = request.session.get('pending_import')
        if not pending_import:
            return redirect('bank_import')

        account = get_object_or_404(Account, id=pending_import['account_id'])
        duplicates = pending_import['duplicates']

        # Convertir fechas de string a date y amounts de string a Decimal
        for movement in duplicates:
            movement['date'] = datetime.strptime(movement['date'], '%Y-%m-%d').date()
            movement['amount'] = Decimal(movement['amount'])
        
        # Filtrar movimientos según selección del usuario
        selected_movements = []
        for movement in duplicates:
            movement_key = f"{movement['date']}_{movement['amount']}"
            if request.POST.get(movement_key) == 'import':
                selected_movements.append(movement)

        if selected_movements:
            success_count = self.import_movements(account, selected_movements)
            messages.success(request, f'Se han importado {success_count} movimientos duplicados seleccionados.')

        # Limpiar datos de sesión
        del request.session['pending_import']
        return redirect('bank_import')
