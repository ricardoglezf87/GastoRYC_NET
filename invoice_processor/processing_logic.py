# invoice_processor/processing_logic.py
from datetime import datetime
import locale
import traceback
from pdf2image import convert_from_bytes, convert_from_path
import pytesseract
from pdf2image import exceptions as pdf2image_exceptions
from PIL import Image
import re
import io
import os
import json 
from django.utils.dateparse import parse_date

from .models import InvoiceDocument, InvoiceType, ExtractedData


script_dir = os.path.dirname(os.path.abspath(__file__))
poppler_bin_path = os.path.join(script_dir, 'poppler', 'Library', 'bin')
pytesseract.pytesseract.tesseract_cmd = os.path.join(script_dir,'tesseract', 'tesseract.exe')

def perform_ocr(file_input): # Renombrar para evitar conflictos si importas algo más
    """
    Realiza OCR en un archivo (ruta, objeto UploadedFile, bytes).
    Devuelve el texto extraído o None si hay error.
    (Adaptado de processing_logic.py para usar poppler_bin_path local)
    """
    global poppler_bin_path # Usar la ruta de poppler definida globalmente
    text = None
    is_pdf = False
    filename = getattr(file_input, 'name', str(type(file_input)))

    try:
        print(f"perform_ocr_local: Iniciando OCR para {filename} (Tipo: {type(file_input)})")

        if isinstance(file_input, str) and file_input.lower().endswith('.pdf'):
            is_pdf = True
        elif hasattr(file_input, 'name') and file_input.name.lower().endswith('.pdf'):
            is_pdf = True

        if is_pdf:
            print("perform_ocr_local: Detectado como PDF.")
            images = []
            if isinstance(file_input, str): # Si es una ruta
                # Usar poppler_path aquí
                images = convert_from_path(file_input, dpi=300, poppler_path=poppler_bin_path) # Ajusta DPI si es necesario
            else: # Si fuera un objeto de archivo (menos probable aquí)
                file_input.seek(0)
                pdf_bytes = file_input.read()
                file_input.seek(0)
                if pdf_bytes:
                     # Usar poppler_path aquí
                     images = convert_from_bytes(pdf_bytes, dpi=300, poppler_path=poppler_bin_path)
                else:
                     print("perform_ocr_local: Error - Contenido del PDF está vacío.")
                     return None

            if not images:
                 print("perform_ocr_local: Error - pdf2image no devolvió imágenes.")
                 return None

            print(f"perform_ocr_local: PDF convertido a {len(images)} imágenes.")
            full_text = ""
            for i, img in enumerate(images):
                print(f"perform_ocr_local: Procesando imagen {i+1}/{len(images)} del PDF...")
                # Asegúrate que Tesseract esté configurado o en el PATH
                config = '--psm 6'
                full_text += pytesseract.image_to_string(img, lang='spa', config=config) + "\n\n"
            text = full_text.strip()

        else: # Asumir que es una imagen
            print("perform_ocr_local: Tratando como imagen.")
            image = Image.open(file_input)
            text = pytesseract.image_to_string(image, lang='spa')

        if text:
            print(f"--- OCR Local Exitoso para '{filename}' ---")
            return text
        else:
            print(f"--- OCR Local para '{filename}' no devolvió texto. ---")
            return ""

    except pytesseract.TesseractNotFoundError:
        print("Error: Tesseract no encontrado localmente. Asegúrate de que está instalado y en el PATH o configura tesseract_cmd.")
        # Ya no se muestra QMessageBox aquí
        return None
    except pdf2image_exceptions.PDFInfoNotInstalledError:
         # Este error ya se maneja en load_document_image, pero por si acaso
         print(f"Error: Poppler (pdfinfo) no encontrado en '{poppler_bin_path}' o no funciona.")
         # Ya no se muestra QMessageBox aquí
         return None
    except Exception as e:
        print(f"Error inesperado durante OCR local para '{filename}': {type(e).__name__}: {e}")
        traceback.print_exc()
        return None

def identify_invoice_type(text):
    """
    Intenta identificar el tipo de factura usando las reglas 'identifier' del JSON.
    """
    if not text:
        return None

    all_types = InvoiceType.objects.filter(extraction_rules__isnull=False)

    for inv_type in all_types:
        rules = inv_type.extraction_rules
        identifier_rule = rules.get('identifier') # Busca la regla de identificación

        if not identifier_rule:
            continue # Pasa al siguiente tipo si no tiene regla de identificación

        rule_type = identifier_rule.get('type')
        rule_value = identifier_rule.get('value')

        if not rule_type or not rule_value:
            continue # Regla incompleta

        try:
            if rule_type == 'keyword' and rule_value in text:
                print(f"Tipo identificado por palabra clave '{rule_value}': {inv_type.name}")
                return inv_type
            elif rule_type == 'regex' and re.search(rule_value, text, re.IGNORECASE | re.MULTILINE):
                print(f"Tipo identificado por regex '{rule_value}': {inv_type.name}")
                return inv_type
            # Añade más tipos de reglas de identificación si es necesario
        except Exception as e:
             print(f"Error aplicando regla de identificación para {inv_type.name}: {e}")


    print("No se pudo identificar el tipo de factura automáticamente usando reglas.")
    return None

def extract_invoice_data(text, rules_or_type):
    """
    Extrae datos usando reglas (de un dict o un InvoiceType) y parsea la fecha.
    """
    if not text:
        print("Texto de entrada faltante.")
        return {}

    rules = None
    invoice_type_name = "Reglas directas" # Nombre por defecto para logs

    # Determinar si recibimos un diccionario de reglas o un objeto InvoiceType
    if isinstance(rules_or_type, InvoiceType):
        if rules_or_type.extraction_rules:
            rules = rules_or_type.extraction_rules
            invoice_type_name = rules_or_type.name
        else:
            print(f"InvoiceType {rules_or_type.name} no tiene reglas de extracción.")
            return {}
    elif isinstance(rules_or_type, dict):
        rules = rules_or_type
    else:
        print("Entrada de reglas inválida (ni dict ni InvoiceType).")
        return {}

    if not rules:
        print("Reglas de extracción vacías o no encontradas.")
        return {}

    extracted = {}

    # --- Extracción de Identificador (Opcional, más útil para prueba local) ---
    identifier_rule = rules.get('identifier')
    if identifier_rule:
        rule_type = identifier_rule.get('type')
        rule_value = identifier_rule.get('value')
        try:
            if rule_type == 'keyword' and rule_value:
                if rule_value in text:
                    extracted['identifier_found'] = rule_value
                    print(f"Identificador encontrado ({invoice_type_name}, keyword '{rule_value}')")
                else:
                    extracted['identifier_found'] = None
                    print(f"Identificador NO encontrado ({invoice_type_name}, keyword '{rule_value}')")
        except Exception as e:
            print(f"Error aplicando regla de identificador ({invoice_type_name}): {e}")

    # --- Extracción de Fecha (con parseo) ---
    date_rule = rules.get('date')
    if date_rule:
        rule_type = date_rule.get('type')
        try:
            if rule_type == 'regex':
                pattern = date_rule.get('pattern')
                if pattern:
                    match = re.search(pattern, text, re.DOTALL | re.IGNORECASE)
                    if match:
                        date_str = match.group(1).strip()
                        print(f"Cadena de fecha encontrada ({invoice_type_name}, regex '{pattern}'): '{date_str}'")
                        parsed_date = parse_date(date_str) # Intenta primero con parse_date

                        if not parsed_date:
                            # --- Configurar locale ANTES de strptime con %B ---
                            original_locale = None # Para restaurarlo después
                            try:
                                # Intenta establecer locale español (ajusta según tu OS)
                                # Para Linux/macOS:
                                original_locale = locale.getlocale(locale.LC_TIME) # Guarda el actual
                                locale.setlocale(locale.LC_TIME, 'es_ES.UTF-8')
                                print("Locale español (es_ES.UTF-8) establecido temporalmente.")
                            except locale.Error:
                                try:
                                    # Para Windows:
                                    original_locale = locale.getlocale(locale.LC_TIME)
                                    locale.setlocale(locale.LC_TIME, 'Spanish_Spain.1252')
                                    print("Locale español (Spanish_Spain.1252) establecido temporalmente.")
                                except locale.Error:
                                    print("Advertencia: No se pudo establecer locale español para parsear fecha con nombre de mes.")
                            # ----------------------------------------------------

                            formats_to_try = [
                                '%d/%m/%Y', '%d.%m.%Y', '%d-%m-%Y',
                                '%d/%m/%y', '%d.%m.%y', '%d-%m-%y',
                                '%d de %B de %Y', # Ahora debería funcionar con locale español
                                '%d %b %Y',
                            ]

                            for fmt in formats_to_try:
                                try:
                                    parsed_date = datetime.strptime(date_str, fmt).date()
                                    break
                                except ValueError:
                                    continue

                            # --- Restaurar locale original ---
                            if original_locale:
                                try:
                                    locale.setlocale(locale.LC_TIME, original_locale)
                                    print("Locale original restaurado.")
                                except locale.Error:
                                     print("Advertencia: No se pudo restaurar el locale original.")
                            # -------------------------------

                        if parsed_date:
                            extracted['invoice_date'] = parsed_date
                            print(f"Fecha parseada ({invoice_type_name}): {parsed_date}")
                        else:
                            print(f"No se pudo parsear la fecha '{date_str}' ({invoice_type_name}).")
        except Exception as e:
            print(f"Error aplicando regla de fecha ({invoice_type_name}): {e}")
            traceback.print_exc()
            # Asegurarse de restaurar locale si hubo error
            if 'original_locale' in locals() and original_locale:
                 try: locale.setlocale(locale.LC_TIME, original_locale)
                 except: pass

    # --- Extracción de Total ---
    total_rule = rules.get('total')
    if total_rule:
        rule_type = total_rule.get('type')
        try:
            if rule_type == 'regex':
                pattern = total_rule.get('pattern')
                if pattern:
                     matches = re.findall(pattern, text, re.IGNORECASE)
                     if matches:
                         amount_str = matches[-1].replace('.', '').replace(',', '.')
                         try:
                             extracted['total_amount'] = float(amount_str) # O Decimal
                             print(f"Total encontrado ({invoice_type_name}, regex '{pattern}'): {extracted['total_amount']}")
                         except ValueError:
                             print(f"Error convirtiendo total ({invoice_type_name}, regex): {matches[-1]}")
        except Exception as e:
            print(f"Error aplicando regla de total ({invoice_type_name}): {e}")

    # ... (Extraer otros campos si los tienes) ...

    print(f"Datos extraídos finales ({invoice_type_name}): {extracted}")
    return extracted

def process_invoice_document(document_id):
    """
    Función principal que orquesta el procesamiento de un InvoiceDocument.
    """
    try:
        doc = InvoiceDocument.objects.get(id=document_id)
        print(f"Procesando Documento ID: {doc.id}, Archivo: {doc.file.name}")
        doc.status = 'PROCESSING'
        doc.save()

        extracted_text = perform_ocr(doc.file.path)
        if extracted_text is None:
            doc.status = 'FAILED'; doc.extracted_text = "Error durante OCR"; doc.save()
            print(f"Fallo OCR para Documento ID: {doc.id}")
            return False, "Error OCR"
        doc.extracted_text = extracted_text

        invoice_type = identify_invoice_type(extracted_text)
        if invoice_type:
            doc.invoice_type = invoice_type
            # --- LLAMAR A LA NUEVA FUNCIÓN ---
            data = extract_invoice_data(extracted_text, invoice_type)
            # -------------------------------
            if data.get('invoice_date') or data.get('total_amount'): # Si se extrajo algo útil
                ExtractedData.objects.update_or_create(
                    document=doc,
                    defaults={
                        'invoice_date': data.get('invoice_date'), # Ya es objeto date
                        'total_amount': data.get('total_amount'),
                    }
                )
                doc.status = 'PROCESSED' # O 'NEEDS_MATCHING' si ese es tu flujo
                print(f"Datos extraídos para Documento ID: {doc.id}")
            else:
                doc.status = 'NEEDS_MAPPING'
                print(f"Tipo identificado pero sin datos extraídos para Documento ID: {doc.id}")
        else:
            doc.status = 'NEEDS_MAPPING'
            print(f"Documento ID: {doc.id} necesita mapeo manual.")

        doc.save()
        print(f"Procesamiento finalizado para Documento ID: {doc.id}. Estado: {doc.status}")
        return True, doc.status
    # ... (manejo de excepciones sin cambios) ...
    except Exception as e:
        print(f"Error procesando documento {document_id}: {e}")
        try:
            doc = InvoiceDocument.objects.get(id=document_id)
            doc.status = 'FAILED'; doc.extracted_text = f"Error inesperado: {e}"; doc.save()
        except: pass
        return False, f"Error inesperado: {e}"
    

