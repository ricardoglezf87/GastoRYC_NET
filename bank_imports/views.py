from django.shortcuts import render, redirect
from django.http import JsonResponse
from django.views.decorators.csrf import csrf_exempt
from GARCA.utils import add_breadcrumb, clear_breadcrumbs
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

class BankImportView(View):
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
