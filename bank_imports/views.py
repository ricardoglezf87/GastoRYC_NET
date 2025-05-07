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
from decimal import Decimal, InvalidOperation
from datetime import datetime
from django.contrib import messages
from django.db import transaction
from django.views import View
from django.utils.decorators import method_decorator
import os # Ensure os is imported
import openpyxl

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
                    debit=credit, # Counterpart transaction is reversed
                    credit=debit  # Counterpart transaction is reversed
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
            
            bank_provider_display_name = dict(form.fields['bank_provider'].choices).get(bank_provider, bank_provider)

            preview_data = None
            import_data = None

            # Refined file extension validation
            _, file_ext_from_name = os.path.splitext(file.name)
            file_extension_lower = file_ext_from_name.lower()

            provider_specific_allowed_extensions = {
                'ing': ['.csv', '.txt'], # Based on original logic for ING
                'edenred': ['.csv', '.txt', '.xlsx'],
                'paypal': ['.csv'],
            }

            current_allowed_extensions = provider_specific_allowed_extensions.get(bank_provider)

            if not current_allowed_extensions:
                messages.error(request, f"Proveedor bancario '{bank_provider_display_name}' no tiene configuración de extensiones permitidas.")
                return render(request, self.template_name, {'form': form})

            if file_extension_lower not in current_allowed_extensions:
                expected_extensions_str = ", ".join(current_allowed_extensions)
                messages.error(request, f"Para {bank_provider_display_name}, el archivo debe ser de tipo {expected_extensions_str}. Archivo proporcionado: {file.name}")
                return render(request, self.template_name, {'form': form})
            
            # Proceed with file processing if extension is valid
            try:
                if bank_provider == 'ing':
                    preview_data, import_data = BankImportView.process_ing_file(file)
                elif bank_provider == 'edenred':
                    preview_data, import_data = BankImportView.process_edenred_file(file)
                    if not preview_data and not import_data: 
                        messages.error(request, "Error al procesar el archivo Edenred. Verifique el formato y la codificación del archivo (UTF-8 o Latin-1 para TXT/CSV, o que sea un XLSX válido).")
                        return render(request, self.template_name, {'form': form})
                elif bank_provider == 'paypal':
                    preview_data, import_data = BankImportView.process_paypal_file(file)
                    if not preview_data and not import_data:
                        messages.error(request, "Error al procesar el archivo PayPal. Verifique que sea un CSV válido (UTF-8 o Latin-1) y que contenga transacciones 'Completado'.")
                        return render(request, self.template_name, {'form': form})
                else:
                    # This case should ideally be caught by form validation, but as a safeguard
                    messages.error(request, f"Proveedor bancario '{bank_provider_display_name}' no soportado para la importación.")
                    return render(request, self.template_name, {'form': form})
            except Exception as e: 
                messages.error(request, f"Error inesperado al procesar el archivo para {bank_provider_display_name}: {e}")
                return render(request, self.template_name, {'form': form})

            if 'import' in request.POST and import_data:
                try:
                    duplicates = []
                    non_duplicates = []
                    
                    for movement in import_data:
                        if self.is_duplicate(account, movement):
                            duplicates.append(movement)
                        else:
                            non_duplicates.append(movement)
                    
                    if non_duplicates:
                        success_count = self.import_movements(account, non_duplicates)
                        if duplicates:
                            messages.success(request, f'Se han importado {success_count} movimientos, pero se han encontrado algunos movimientos duplicados')
                        else:
                            messages.success(request, f'Se han importado {success_count} movimientos')
                    
                    if duplicates:
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
                except Exception as e:
                    messages.error(request, f"Error durante el proceso de importación o verificación de duplicados: {e}")
                    return render(request, self.template_name, {'form': form})

        return render(request, self.template_name, {'form': form})

    def is_duplicate(self, account, movement):
        debit_amount = movement['amount'] if movement['amount'] > 0 else 0
        credit_amount = abs(movement['amount']) if movement['amount'] < 0 else 0
        
        return Transaction.objects.filter(
            account=account,
            entry__date=movement['date'],
            debit=debit_amount,
            credit=credit_amount
        ).exists()

    @staticmethod
    def process_ing_file(file):
        content = file.read().decode('utf-8') 
        csv_data = [] 
        import_data = [] 
        
        csv_reader = csv.reader(io.StringIO(content), delimiter=';')
        
        for _ in range(5):
            next(csv_reader, None)
        
        for row in csv_reader:
            if len(row) >= 6:
                try:
                    date_str = row[0]
                    category = row[1]
                    subcategory = row[2]
                    description = row[3]
                    comment = row[4]
                    amount_str = row[5].replace('.', '').replace(',', '.')
                    
                    if date_str:
                        date = datetime.strptime(date_str, '%d/%m/%Y').date()
                        
                        if amount_str:
                            try:
                                amount = Decimal(amount_str)
                                
                                csv_data.append({
                                    'date': date,
                                    'category': f"{category} - {subcategory}",
                                    'description': description,
                                    'amount': amount 
                                })
                                
                                import_data.append({
                                    'date': date,
                                    'category': category,
                                    'subcategory': subcategory,
                                    'description': description,
                                    'comment': comment,
                                    'amount': amount 
                                })
                            except InvalidOperation:
                                pass 
                except Exception:
                    pass 

        return csv_data, import_data

    @staticmethod
    def _parse_edenred_row(row_parts):
        """Helper para parsear una fila de datos de Edenred (común a TXT y XLSX)."""
        if len(row_parts) >= 3:
            date_str = str(row_parts[0]).strip() # Asegurar que es string para .xlsx
            description = str(row_parts[1]).strip()
            amount_str = str(row_parts[2]).strip()

            if not date_str and not description and not amount_str:
                return None
            
            parsed_date = None
            try:
                if isinstance(row_parts[0], datetime): # Si ya es datetime (de openpyxl)
                    parsed_date = row_parts[0].date()
                elif ' ' in date_str: 
                    parsed_date = datetime.strptime(date_str, '%Y-%m-%d %H:%M:%S').date()
                else: 
                    parsed_date = datetime.strptime(date_str, '%Y-%m-%d').date()
            except ValueError:
                return None

            amount_decimal = None
            try:
                amount_decimal = Decimal(amount_str.replace(',', '.'))
            except InvalidOperation:
                return None
            
            final_amount = amount_decimal if "RECARGA" in description.upper() else -amount_decimal

            return {
                'date': parsed_date,
                'description': description,
                'amount': final_amount
            }
        return None

    @staticmethod
    def _process_edenred_text(file):
        preview_data = []
        import_data = []
        content = None
        try:
            content = file.read().decode('utf-8')
        except UnicodeDecodeError:
            file.seek(0) 
            try:
                content = file.read().decode('latin-1')
            except UnicodeDecodeError:
                return [], [] 

        data_io = io.StringIO(content)
        lines = data_io.readlines()
        
        data_start_index = -1
        expected_header_cols = ["Fecha", "Detalle movimiento", "Importe"]

        for i, line_content in enumerate(lines):
            cols = [col.strip() for col in line_content.split('\t')]
            if len(cols) >= len(expected_header_cols) and \
               cols[:len(expected_header_cols)] == expected_header_cols:
                data_start_index = i + 1
                break
        
        if data_start_index == -1:
            return [], []

        for i in range(data_start_index, len(lines)):
            line = lines[i].strip()
            if not line: 
                continue
            row_parts = [part.strip() for part in line.split('\t')]
            parsed_movement = BankImportView._parse_edenred_row(row_parts)
            if parsed_movement:
                preview_data.append(parsed_movement)
                import_data.append(parsed_movement)
        return preview_data, import_data

    @staticmethod
    def _process_edenred_xlsx(file):
        preview_data = []
        import_data = []
        try:
            # Asegurarse de que el puntero del archivo esté al inicio para openpyxl
            file.seek(0)
            workbook = openpyxl.load_workbook(file, data_only=True) # data_only=True para obtener valores de fórmulas
            sheet = workbook.active
        except Exception as e:
            # Consider logging the error e instead of printing
            return [], []

        data_start_index = -1
        expected_header_cols = ["Fecha", "Detalle movimiento", "Importe"]
        
        # Iterar sobre las filas para encontrar el encabezado
        # openpyxl row y column index son 1-based
        for i, row in enumerate(sheet.iter_rows()):
            # Obtener los valores de las celdas, convirtiendo a string y quitando espacios
            # Tomar solo las primeras N celdas necesarias para el encabezado
            cols = [str(cell.value).strip() if cell.value is not None else "" for cell in row[:len(expected_header_cols)]]
            if cols == expected_header_cols:
                data_start_index = i + 1 # El índice de la fila de datos es el siguiente al encabezado (0-based para enumerate)
                break
        
        if data_start_index == -1:
            return [], [] # Encabezado no encontrado

        # Procesar filas de datos
        # sheet.iter_rows() devuelve tuplas de celdas. min_row es 1-based.
        for row_idx, row_cells in enumerate(sheet.iter_rows(min_row=data_start_index + 1)): # +1 porque min_row es 1-based
            # Extraer valores de las celdas. openpyxl puede devolver directamente tipos como datetime.
            row_values = [cell.value for cell in row_cells]
            
            # Verificar si la fila está esencialmente vacía para evitar procesar filas en blanco al final
            if all(value is None or str(value).strip() == "" for value in row_values[:3]): # Chequear las primeras 3 columnas
                continue

            parsed_movement = BankImportView._parse_edenred_row(row_values) # Usar el helper
            if parsed_movement:
                preview_data.append(parsed_movement)
                import_data.append(parsed_movement)
                
        return preview_data, import_data

    @staticmethod
    def process_edenred_file(file):
        """Procesa un archivo de Edenred (TXT, CSV delimitado por tabs, o XLSX)."""
        file_name_lower = file.name.lower()
        if file_name_lower.endswith('.xlsx'):
            return BankImportView._process_edenred_xlsx(file)
        elif file_name_lower.endswith(('.txt', '.csv')): # Asumir CSV es como TXT para Edenred
            return BankImportView._process_edenred_text(file)
        return [], [] # Tipo de archivo no soportado para Edenred

    @staticmethod
    def process_paypal_file(file):
        preview_data = []
        import_data = []
        
        try:
            file.seek(0)
            content = file.read().decode('utf-8-sig') # Handles UTF-8 with BOM
        except UnicodeDecodeError:
            file.seek(0)
            try:
                content = file.read().decode('latin-1') # Fallback encoding
            except UnicodeDecodeError:
                # Could log this error
                return [], [] 

        csv_file_like = io.StringIO(content)
        csv_reader = csv.reader(csv_file_like)
        
        try:
            header = next(csv_reader) # Skip header row
            # Optional: Add header validation if needed
            # expected_header_min_fields = ["Fecha", "Nombre", "Estado", "Importe"]
            # if not all(field in header for field in expected_header_min_fields):
            #     return [], [] # Header mismatch
        except StopIteration:
            return [], [] # Empty file

        for row_number, row in enumerate(csv_reader, start=2): # For logging/debugging
            if len(row) < 8: # PayPal CSV has "Importe" at index 7
                # Log or skip short rows
                continue

            try:
                date_str = row[0].strip()
                name_description = row[3].strip()  # "Nombre" field
                status = row[5].strip()
                amount_str = row[7].strip()  # "Importe" field

                if status.lower() != "completado":
                    continue # Process only completed transactions

                if not date_str or not name_description or not amount_str:
                    continue # Skip rows with missing essential data
                
                parsed_date = datetime.strptime(date_str, '%d/%m/%Y').date()
                amount = Decimal(amount_str.replace(',', '.')) # PayPal uses comma as decimal separator
                
                movement_data = {
                    'date': parsed_date,
                    'description': name_description,
                    'amount': amount,
                    'category': '', # Placeholder, adapt if PayPal provides categories
                    'subcategory': '', # Placeholder
                    'comment': '' # Placeholder
                }
                preview_data.append(movement_data)
                import_data.append(movement_data)
            except (ValueError, InvalidOperation):
                # Log row_number and row data for debugging if parsing fails
                pass 
        return preview_data, import_data

@method_decorator(csrf_exempt, name='dispatch')
class BankImportPreviewView(View):
    def post(self, request):
        form = BankImportForm(request.POST, request.FILES) 
        if form.is_valid():
            bank_provider = form.cleaned_data['bank_provider']
            file = request.FILES['file'] 
            bank_provider_display_name = dict(form.fields['bank_provider'].choices).get(bank_provider, bank_provider)
            
            preview_data = None
            
            # Refined file extension validation
            _, file_ext_from_name = os.path.splitext(file.name)
            file_extension_lower = file_ext_from_name.lower()

            provider_specific_allowed_extensions = {
                'ing': ['.csv', '.txt'],
                'edenred': ['.csv', '.txt', '.xlsx'],
                'paypal': ['.csv'],
            }
            current_allowed_extensions = provider_specific_allowed_extensions.get(bank_provider)

            if not current_allowed_extensions:
                 return JsonResponse({'success': False, 'error': f"Proveedor bancario '{bank_provider_display_name}' no tiene configuración de extensiones."})

            if file_extension_lower not in current_allowed_extensions:
                expected_extensions_str = ", ".join(current_allowed_extensions)
                return JsonResponse({'success': False, 'error': f"Para {bank_provider_display_name}, el archivo debe ser de tipo {expected_extensions_str}. Archivo: {file.name}"})

            try:
                if bank_provider == 'ing':
                    preview_data, _ = BankImportView.process_ing_file(file)
                elif bank_provider == 'edenred':
                    preview_data, _ = BankImportView.process_edenred_file(file)
                    if not preview_data and not _: 
                         return JsonResponse({'success': False, 'error': 'Error al procesar el archivo Edenred. Verifique el formato y codificación.'})
                elif bank_provider == 'paypal':
                    preview_data, _ = BankImportView.process_paypal_file(file)
                    if not preview_data and not _:
                        return JsonResponse({'success': False, 'error': "Error al procesar el archivo PayPal. Verifique que sea un CSV válido y contenga transacciones 'Completado'."})
                else:
                    return JsonResponse({'success': False, 'error': f"Proveedor bancario '{bank_provider_display_name}' no soportado para vista previa."})
                
                if preview_data is not None: # preview_data can be an empty list if file is valid but has no data
                    serializable_preview_data = []
                    for item in preview_data:
                        item_copy = item.copy()
                        item_copy['date'] = item_copy['date'].strftime('%Y-%m-%d')
                        item_copy['amount'] = str(item_copy['amount'])
                        serializable_preview_data.append(item_copy)
                    return JsonResponse({'success': True, 'preview_data': serializable_preview_data})
                else: # Should not be reached if process methods return ([],[]) on error, caught by earlier checks
                    return JsonResponse({'success': False, 'error': 'No se pudieron generar datos de vista previa.'})

            except Exception as e: 
                return JsonResponse({'success': False, 'error': f'Error al procesar el archivo para vista previa ({bank_provider_display_name}): {e}'})
        
        errors = form.errors.as_json()
        return JsonResponse({'success': False, 'errors': errors})

class BankImportDuplicatesView(ImportMovementsMixin, View):
    template_name = 'import_duplicates.html'

    def get(self, request):
        pending_import = request.session.get('pending_import')
        if not pending_import:
            return redirect('bank_import')

        # Convert dates from string to date and amounts from string to Decimal for display
        duplicates_for_template = []
        for movement_str_data in pending_import['duplicates']:
            movement_copy = movement_str_data.copy()
            movement_copy['date'] = datetime.strptime(movement_str_data['date'], '%Y-%m-%d').date()
            movement_copy['amount'] = Decimal(movement_str_data['amount'])
            duplicates_for_template.append(movement_copy)

        return render(request, self.template_name, {
            'duplicates': duplicates_for_template
        })

    def post(self, request):
        pending_import = request.session.get('pending_import')
        if not pending_import:
            return redirect('bank_import')

        account = get_object_or_404(Account, id=pending_import['account_id'])
        
        # Original duplicates data from session (strings for date/amount)
        session_duplicates_str = pending_import['duplicates']
        
        # Convert back to Decimal/date for processing, matching the structure used by import_movements
        movements_to_process = []
        for movement_str_data in session_duplicates_str:
            movement_data = movement_str_data.copy() # Operate on a copy
            movement_data['date'] = datetime.strptime(movement_str_data['date'], '%Y-%m-%d').date()
            movement_data['amount'] = Decimal(movement_str_data['amount'])
            movements_to_process.append(movement_data)

        selected_movements = []
        for movement in movements_to_process:
            # Key for POST data should match how it's generated in the template
            # The template likely uses date and amount to create a unique key for each checkbox
            # Example key: f"{movement['date'].strftime('%Y-%m-%d')}_{str(movement['amount'])}"
            # Let's assume the template creates a key like "YYYY-MM-DD_AMOUNT"
            # The original string versions from session_duplicates_str are good for key matching
            
            # Find the corresponding string version to build the key as the template would
            original_str_movement = next(
                (m for m in session_duplicates_str 
                 if m['date'] == movement['date'].strftime('%Y-%m-%d') and 
                    m['amount'] == str(movement['amount'])), None)

            if original_str_movement:
                movement_key = f"{original_str_movement['date']}_{original_str_movement['amount']}"
                if request.POST.get(movement_key) == 'import':
                    selected_movements.append(movement) # Add the processed movement (with date/Decimal objects)

        if selected_movements:
            success_count = self.import_movements(account, selected_movements)
            messages.success(request, f'Se han importado {success_count} movimientos duplicados seleccionados.')

        if 'pending_import' in request.session:
            del request.session['pending_import']
        return redirect('bank_import')
