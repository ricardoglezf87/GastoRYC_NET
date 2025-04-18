from datetime import timedelta
import os
from django.shortcuts import get_object_or_404
from django.http import JsonResponse
from django.views.decorators.http import require_POST
from django.views.decorators.csrf import csrf_exempt
from django.contrib.contenttypes.models import ContentType
from .models import Attachment
from accounts.models import Account
from entries.models import Entry
import json
import fitz
from django.utils.dateparse import parse_date
from django.db import transaction as db_transaction
from decimal import Decimal, InvalidOperation
import re
from django.shortcuts import render, get_object_or_404
from .forms import MultiplePdfUploadForm

try:
    # Intenta importar dateutil para usarlo si está disponible
    from dateutil.parser import parse as parse_dateutil
    DATEUTIL_AVAILABLE = True
    print("INFO: python-dateutil encontrado y disponible.") # Log opcional
except ImportError:
    DATEUTIL_AVAILABLE = False
    print("ADVERTENCIA: python-dateutil no está instalado. El parseo de fechas será menos flexible.") 


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


def procesar_pdf(pdf_contenido):
    """
    Extrae la fecha principal y TODOS los importes válidos del PDF.
    Devuelve una lista de diccionarios {'fecha': main_date, 'importe': amount}
    por cada importe encontrado, si se halla al menos una fecha.
    """
    all_dates = []
    all_amounts = []
    main_date = None
    results = [] # Lista para los resultados a devolver

    try:
        doc = fitz.open(stream=pdf_contenido, filetype="pdf")
        texto_completo = ""
        for page_num, page in enumerate(doc):
            texto_completo += page.get_text("text")
            # print(f"--- Texto Página {page_num + 1} ---\n{page.get_text('text')}\n--------------------") # Descomentar para depurar texto
        doc.close()

        regex_fecha = r'\b(\d{1,2}[/-]\d{1,2}[/-]\d{2,4})\b'
        regex_importe = r'([-+]?\s*\b\d{1,3}(?:[.,]\d{3})*[,.]\d{2}\b)\s*€?'

        # 1. Encontrar todas las fechas válidas y seleccionar la principal (primera)
        print("\n--- Buscando Fechas ---")
        found_date_strings = re.findall(regex_fecha, texto_completo)
        for fecha_str in found_date_strings:
            # ... (lógica de parseo de fecha igual que antes, actualiza all_dates) ...
            print(f"  Encontrado string de fecha: '{fecha_str}'")
            fecha_obj = None
            try:
                if DATEUTIL_AVAILABLE:
                    try:
                        fecha_obj = parse_dateutil(fecha_str, dayfirst=True).date()
                        # print(f"    -> Parseado con dateutil: {fecha_obj}")
                    except ValueError: pass
                if not fecha_obj:
                    fecha_obj = parse_date(fecha_str)
                    # if fecha_obj: print(f"    -> Parseado con parse_date (Django): {fecha_obj}")

                if fecha_obj:
                    all_dates.append(fecha_obj)
                # else:
                #     print(f"    -> No se pudo parsear '{fecha_str}'")
            except Exception as e:
                print(f"    -> Error inesperado parseando '{fecha_str}': {e}")

        if all_dates:
            main_date = all_dates[0]
            print(f"\nFecha Principal seleccionada: {main_date}")
        else:
            print("\nNo se encontraron fechas válidas en el PDF. No se puede continuar.")
            return [] # Sin fecha, no podemos asociar importes

        # 2. Encontrar todos los importes válidos
        print("\n--- Buscando Importes ---")
        found_amount_strings = re.findall(regex_importe, texto_completo)
        for importe_str in found_amount_strings:
             # ... (lógica de parseo de importe igual que antes, actualiza all_amounts) ...
            print(f"  Encontrado string de importe: '{importe_str}'")
            try:
                importe_limpio = importe_str.replace('€', '').replace('+', '').strip()
                es_negativo = '-' in importe_limpio
                importe_limpio = importe_limpio.replace('-', '').strip()
                if ',' in importe_limpio and '.' in importe_limpio:
                    importe_limpio = importe_limpio.replace('.', '')
                    importe_limpio = importe_limpio.replace(',', '.')
                elif ',' in importe_limpio:
                    importe_limpio = importe_limpio.replace(',', '.')
                importe = Decimal(importe_limpio)
                if es_negativo: importe = -importe
                # print(f"    -> Parseado como Decimal: {importe}")
                all_amounts.append(importe)
            except (ValueError, InvalidOperation, TypeError) as e:
                print(f"    -> Error parseando importe '{importe_str}': {e}")

        # 3. Crear la lista de resultados combinando la fecha principal con cada importe
        if all_amounts:
            print(f"\nSe encontraron {len(all_amounts)} importes válidos.")
            for amount in all_amounts:
                results.append({
                    'fecha': main_date,
                    'importe': amount,
                    # Podríamos intentar añadir contexto si supiéramos la línea original
                    'descripcion_linea': f'Importe {amount} encontrado en PDF'
                })
            print(f"-> Devolviendo {len(results)} posibles combinaciones fecha/importe.")
            return results
        else:
            print("\nNo se encontraron importes válidos en el PDF.")
            return []

    except Exception as e:
        print(f"Error general al leer o procesar el PDF: {e}")
        return []


# -----------------------------------------------------------------------------
# Función Universal para Buscar Coincidencias (SIN cuenta principal)
# -----------------------------------------------------------------------------
def find_matching_entry_universal(extracted_date, extracted_amount, extracted_desc=None, date_tolerance_days=2):
    """
    Busca un Entry único que coincida con fecha e importe en *toda* la base de datos.
    Devuelve un diccionario: {'status': 'found'/'not_found'/'multiple', 'entry': Entry or None, 'candidates': list_of_entries}
    """
    if not extracted_date or extracted_amount is None:
        return {'status': 'error', 'entry': None, 'candidates': [], 'message': 'Datos insuficientes (fecha o importe)'}

    min_date = extracted_date - timedelta(days=date_tolerance_days)
    max_date = extracted_date + timedelta(days=date_tolerance_days)
    amount_abs = abs(extracted_amount)

    # Buscar Entradas que tengan fecha cercana y *alguna* transacción con el importe correcto
    potential_entries_query = Entry.objects.filter(
        date__range=(min_date, max_date)
    ).prefetch_related('transactions', 'transactions__account') # Optimización

    # Filtrar por importe (débito o crédito) en CUALQUIERA de sus transacciones
    if extracted_amount < 0: # Buscamos un débito
        potential_entries_query = potential_entries_query.filter(transactions__debit=amount_abs)
    else: # Buscamos un crédito
        potential_entries_query = potential_entries_query.filter(transactions__credit=amount_abs)

    # Ejecutar y obtener entradas únicas
    potential_entries = list(potential_entries_query.distinct())

    # --- Lógica de Resultado ---
    if len(potential_entries) == 1:
        # ¡Coincidencia única encontrada!
        return {'status': 'found', 'entry': potential_entries[0], 'candidates': potential_entries}
    elif len(potential_entries) > 1:
        # Múltiples coincidencias. Podrías intentar desempatar aquí usando 'extracted_desc'.
        # Por ahora, lo marcamos como ambiguo.
        print(f"ADVERTENCIA: Múltiples Entries encontrados para Fecha ~{extracted_date}, Importe {extracted_amount}. IDs: {[e.id for e in potential_entries]}")
        return {'status': 'multiple', 'entry': None, 'candidates': potential_entries}
    else:
        # No se encontró ninguna coincidencia.
        return {'status': 'not_found', 'entry': None, 'candidates': []}

def asociar_pdfs_view(request):
    if request.method == 'POST':
        form = MultiplePdfUploadForm(request.POST, request.FILES)
        if form.is_valid():
            pdf_file = form.cleaned_data['pdf_file']
            processed_files_summary = {}

            if pdf_file:
                pdf_filename = pdf_file.name
                pdf_key = os.path.basename(pdf_filename)
                # Inicializa el resumen para este archivo
                processed_files_summary[pdf_key] = {'associated_entry_id': None, 'errors': [], 'matches_tried': []} # Cambiado 'matches' a 'matches_tried'

                try:
                    pdf_content = pdf_file.read()
                    # procesar_pdf ahora devuelve una lista de posibles items (fecha, importe)
                    extracted_items = procesar_pdf(pdf_content)
                    print(f"Items extraídos del PDF para buscar coincidencias: {extracted_items}")

                    if not extracted_items:
                        processed_files_summary[pdf_key]['errors'].append("No se extrajeron datos válidos (fecha/importe) del PDF.")
                    else:
                        found_match_for_file = False # Flag para saber si ya asociamos este PDF
                        # Iterar sobre CADA posible combinación fecha/importe extraída
                        for item in extracted_items:
                            # Si ya encontramos una coincidencia y asociamos este PDF, no seguir buscando
                            if found_match_for_file:
                                break

                            print(f"  -> Intentando buscar coincidencia para: Fecha={item.get('fecha')}, Importe={item.get('importe')}")
                            match_result = find_matching_entry_universal(
                                item.get('fecha'),
                                item.get('importe'),
                                item.get('descripcion_linea') # Pasa la descripción si la tienes
                            )

                            entry_match = match_result.get('entry')
                            match_status = match_result.get('status')
                            candidate_entries = match_result.get('candidates', [])

                            # Guardar el resultado de este intento en el resumen
                            match_attempt_info = {
                                'status': match_status,
                                'entry': entry_match,
                                'accounts': [], # Se llenará si hay éxito
                                'extracted_data': item,
                                'candidate_entries': candidate_entries,
                                'error_detail': None
                            }

                            if match_status == 'found' and entry_match:
                                print(f"    --> ¡Coincidencia ÚNICA encontrada! Entry ID: {entry_match.id}")
                                try:
                                    content_type = ContentType.objects.get_for_model(Entry)
                                    existing_attachment = Attachment.objects.filter(
                                        content_type=content_type,
                                        object_id=entry_match.id,
                                    ).exists()

                                    with db_transaction.atomic():
                                        pdf_file.seek(0)
                                        Attachment.objects.create(
                                            file=pdf_file,
                                            content_type=content_type,
                                            object_id=entry_match.id,
                                            description=f"Auto-adjuntado: {pdf_filename} (Coincidencia: {item.get('fecha')} / {item.get('importe')})"
                                        )

                                    status_display = "Asociado"
                                    if existing_attachment:
                                        status_display += " (Entrada ya tenía otros adjuntos)"

                                    # Actualizar el estado del intento y añadir cuentas
                                    match_attempt_info['status'] = status_display
                                    match_attempt_info['accounts'] = list(Account.objects.filter(transaction__entry=entry_match).distinct())

                                    # Marcar que ya asociamos este archivo y actualizar resumen principal
                                    processed_files_summary[pdf_key]['associated_entry_id'] = entry_match.id
                                    found_match_for_file = True # ¡Importante! Detiene la búsqueda para este PDF
                                    print(f"    --> PDF '{pdf_filename}' asociado a Entry {entry_match.id}. Deteniendo búsqueda para este archivo.")

                                except Exception as e:
                                    error_msg = f"Error al adjuntar a Entry {entry_match.id}: {e}"
                                    print(f"    --> ERROR: {error_msg}")
                                    processed_files_summary[pdf_key]['errors'].append(error_msg)
                                    match_attempt_info['status'] = 'Error al adjuntar'
                                    match_attempt_info['error_detail'] = str(e)

                            elif match_status == 'multiple':
                                print(f"    --> Múltiples coincidencias encontradas para este importe.")
                                # No hacemos nada, solo registramos el intento
                            elif match_status == 'not_found':
                                print(f"    --> No se encontraron coincidencias para este importe.")
                                # No hacemos nada, solo registramos el intento
                            elif match_status == 'error':
                                processed_files_summary[pdf_key]['errors'].append(match_result.get('message', 'Error en búsqueda'))
                                # No hacemos nada más, solo registramos el intento

                            # Añadir el resultado de este intento a la lista de intentos del resumen
                            processed_files_summary[pdf_key]['matches_tried'].append(match_attempt_info)

                        # Después de probar todos los items, verificar si se asoció
                        if not found_match_for_file and not processed_files_summary[pdf_key]['errors']:
                             # Si no hubo errores graves pero tampoco se asoció
                             processed_files_summary[pdf_key]['errors'].append("No se encontró una coincidencia única para ningún importe del PDF.")

                except Exception as e:
                    # ... (manejo de error general del PDF) ...
                    error_msg = f"Error general al procesar PDF {pdf_filename}: {e}"
                    print(error_msg)
                    if pdf_key not in processed_files_summary:
                         processed_files_summary[pdf_key] = {'associated_entry_id': None, 'errors': [], 'matches_tried': []}
                    processed_files_summary[pdf_key]['errors'].append(error_msg)

            # Convertir a lista para el template
            results_list = [{'pdf_name': k, **v} for k, v in processed_files_summary.items()]

            # Renderizar el resumen
            return render(request, 'asociar_pdf_summary.html', { # Asegúrate que la ruta sea correcta
                'results': results_list,
            })
        # else: # Formulario no válido
        #     pass # Renderizar abajo

    # Método GET
    form = MultiplePdfUploadForm()
    return render(request, 'asociar_pdf_upload.html', {'form': form}) # Asegúrate que la ruta sea correcta