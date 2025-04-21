# invoice_processor/processing_logic.py
import traceback
from pdf2image import convert_from_bytes, convert_from_path
import pytesseract
from pdf2image import exceptions as pdf2image_exceptions
from PIL import Image
import re
import io

import json 
from .models import InvoiceDocument, InvoiceType, ExtractedData
# Importa aquí librerías para PDF si trabajas con ellos (ej. pdf2image)
# from pdf2image import convert_from_path # Ejemplo

# --- Configuración de Tesseract (Ajusta la ruta si es necesario) ---
# Si tesseract no está en tu PATH del sistema, descomenta y ajusta la línea:
# pytesseract.pytesseract.tesseract_cmd = r'C:\Program Files\Tesseract-OCR\tesseract.exe' # Ejemplo Windows
# pytesseract.pytesseract.tesseract_cmd = '/usr/bin/tesseract' # Ejemplo Linux

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
        print(f"perform_ocr_local: Iniciando OCR para '{filename}' (Tipo: {type(file_input)})")

        if isinstance(file_input, str) and file_input.lower().endswith('.pdf'):
            is_pdf = True
        elif hasattr(file_input, 'name') and file_input.name.lower().endswith('.pdf'):
            is_pdf = True

        if is_pdf:
            print("perform_ocr_local: Detectado como PDF.")
            images = []
            if isinstance(file_input, str): # Si es una ruta
                # Usar poppler_path aquí
                images = convert_from_path(file_input, dpi=200, poppler_path=poppler_bin_path) # Ajusta DPI si es necesario
            else: # Si fuera un objeto de archivo (menos probable aquí)
                file_input.seek(0)
                pdf_bytes = file_input.read()
                file_input.seek(0)
                if pdf_bytes:
                     # Usar poppler_path aquí
                     images = convert_from_bytes(pdf_bytes, dpi=200, poppler_path=poppler_bin_path)
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
                full_text += pytesseract.image_to_string(img, lang='spa') + "\n\n"
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

def extract_data(text, rules): # Renombrar y adaptar
    """
    Extrae datos usando las reglas proporcionadas.
    (Adaptado de extract_data_based_on_type)
    """
    if not text or not rules:
        print("Texto o reglas de extracción faltantes.")
        return {}

    extracted = {}
    # No necesitamos invoice_type, usamos directamente el diccionario 'rules'

    # --- Ejemplo de Extracción de Fecha ---
    date_rule = rules.get('date')
    if date_rule:
        rule_type = date_rule.get('type')
        try:
            if rule_type == 'regex':
                pattern = date_rule.get('pattern')
                if pattern:
                    match = re.search(pattern, text)
                    if match:
                        # Devolvemos como string para la prueba
                        extracted['invoice_date'] = match.group(0)
                        print(f"Fecha encontrada (local, regex '{pattern}'): {extracted['invoice_date']}")
            # Añadir más tipos de reglas si las tienes...
        except Exception as e:
            print(f"Error aplicando regla de fecha local: {e}")

    # --- Ejemplo de Extracción de Total ---
    total_rule = rules.get('total')
    if total_rule:
        rule_type = total_rule.get('type')
        try:
            if rule_type == 'regex':
                pattern = total_rule.get('pattern')
                if pattern:
                     matches = re.findall(pattern, text, re.IGNORECASE)
                     if matches:
                         # Limpiar y convertir el último encontrado
                         amount_str = matches[-1].replace('.', '').replace(',', '.')
                         try:
                             # Devolvemos como float para la prueba
                             extracted['total_amount'] = float(amount_str)
                             print(f"Total encontrado (local, regex '{pattern}'): {extracted['total_amount']}")
                         except ValueError:
                             print(f"Error convirtiendo total local (regex): {matches[-1]}")
            # Añadir más tipos de reglas si las tienes...
        except Exception as e:
            print(f"Error aplicando regla de total local: {e}")

    # ... Extraer otros campos ...

    print(f"Datos extraídos locales finales: {extracted}")
    return extracted

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

def extract_data_based_on_type(text, invoice_type):
    """
    Extrae datos usando las reglas definidas en extraction_rules del InvoiceType.
    """
    if not text or not invoice_type or not invoice_type.extraction_rules:
        print("Texto, tipo de factura o reglas de extracción faltantes.")
        return {}

    extracted = {}
    rules = invoice_type.extraction_rules

    # --- Ejemplo de Extracción de Fecha ---
    date_rule = rules.get('date')
    if date_rule:
        rule_type = date_rule.get('type')
        try:
            if rule_type == 'regex':
                pattern = date_rule.get('pattern')
                if pattern:
                    match = re.search(pattern, text)
                    if match:
                        # Aquí necesitarás parsear la fecha encontrada
                        # Deberías guardar el formato esperado también en las reglas
                        # o intentar varios formatos comunes.
                        extracted['invoice_date_str'] = match.group(0) # Guardar como texto por ahora
                        print(f"Fecha encontrada (regex '{pattern}'): {extracted['invoice_date_str']}")
                        # TODO: Parsear a objeto Date
            elif rule_type == 'keyword_proximity':
                keyword = date_rule.get('keyword')
                lines_after = date_rule.get('lines_after', 0)
                # Lógica para buscar keyword y extraer de línea(s) siguientes
                # (Esta lógica puede ser compleja)
                pass
            elif rule_type == 'coordinates':
                coords = date_rule.get('value') # Asume [x1, y1, x2, y2]
                # Necesitarías pytesseract.image_to_data para obtener bounding boxes
                # y encontrar el texto dentro de esas coordenadas. Requiere la imagen original.
                pass
            # Añadir más tipos de reglas de fecha
        except Exception as e:
            print(f"Error aplicando regla de fecha para {invoice_type.name}: {e}")


    # --- Ejemplo de Extracción de Total ---
    total_rule = rules.get('total')
    if total_rule:
        rule_type = total_rule.get('type')
        try:
            if rule_type == 'regex':
                pattern = total_rule.get('pattern')
                # Ejemplo: Buscar 'TOTAL' seguido de números con decimales
                # pattern = r'(?:TOTAL|IMPORTE)\s*:?\s*([\d.,]+)'
                if pattern:
                     # Buscar todas las coincidencias y quizás tomar la última o la mayor
                     matches = re.findall(pattern, text, re.IGNORECASE)
                     if matches:
                         # Limpiar y convertir el último encontrado (o el más probable)
                         amount_str = matches[-1].replace('.', '').replace(',', '.')
                         try:
                             extracted['total_amount'] = float(amount_str)
                             print(f"Total encontrado (regex '{pattern}'): {extracted['total_amount']}")
                         except ValueError:
                             print(f"Error convirtiendo total (regex): {matches[-1]}")
            elif rule_type == 'keyword_proximity':
                 keyword = total_rule.get('keyword', 'TOTAL') # Palabra clave por defecto
                 # Lógica para buscar keyword y el número más cercano
                 pass
            elif rule_type == 'coordinates':
                 coords = total_rule.get('value')
                 # Lógica con image_to_data
                 pass
             # Añadir más tipos de reglas de total
        except Exception as e:
            print(f"Error aplicando regla de total para {invoice_type.name}: {e}")

    # ... Extraer otros campos de manera similar ...

    # --- Limpieza y Formateo Final ---
    # Convierte la fecha string a objeto Date si se encontró
    if 'invoice_date_str' in extracted:
         from django.utils import parse_date # Necesitarás crear una función robusta de parseo
         parsed_date = parse_date(extracted['invoice_date_str'])
         if parsed_date:
             extracted['invoice_date'] = parsed_date
         del extracted['invoice_date_str'] # Elimina la versión string

    print(f"Datos extraídos finales: {extracted}")
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

        # 1. Realizar OCR
        # Usamos doc.file.path para obtener la ruta al archivo guardado por Django
        extracted_text = perform_ocr(doc.file.path)
        if extracted_text is None:
            doc.status = 'FAILED'
            doc.extracted_text = "Error durante OCR"
            doc.save()
            print(f"Fallo OCR para Documento ID: {doc.id}")
            return False, "Error OCR"

        doc.extracted_text = extracted_text

        # 2. Intentar identificar el tipo
        invoice_type = identify_invoice_type(extracted_text)

        if invoice_type:
            doc.invoice_type = invoice_type
            # 3. Extraer datos si se identificó el tipo
            data = extract_data_based_on_type(extracted_text, invoice_type)

            if data:
                # Guardar datos extraídos
                ExtractedData.objects.update_or_create(
                    document=doc,
                    defaults={
                        'invoice_date': data.get('invoice_date'), # Asegúrate que esté en formato Date
                        'total_amount': data.get('total_amount'),
                        # ... otros campos ...
                    }
                )
                doc.status = 'PROCESSED'
                print(f"Datos extraídos para Documento ID: {doc.id}")
            else:
                # Se identificó pero no se pudieron extraer datos (quizás reglas incompletas)
                doc.status = 'NEEDS_MAPPING' # O podrías poner 'PROCESSED' si la identificación es suficiente
                print(f"Tipo identificado pero sin datos extraídos para Documento ID: {doc.id}")

        else:
            # No se pudo identificar el tipo
            doc.status = 'NEEDS_MAPPING'
            print(f"Documento ID: {doc.id} necesita mapeo manual.")

        doc.save()
        print(f"Procesamiento finalizado para Documento ID: {doc.id}. Estado: {doc.status}")
        return True, doc.status

    except InvoiceDocument.DoesNotExist:
        print(f"Error: Documento con ID {document_id} no encontrado.")
        return False, "Documento no encontrado"
    except Exception as e:
        print(f"Error procesando documento {document_id}: {e}")
        # Marcar como fallido si ocurre un error inesperado
        try:
            doc = InvoiceDocument.objects.get(id=document_id)
            doc.status = 'FAILED'
            doc.extracted_text = f"Error inesperado: {e}"
            doc.save()
        except InvoiceDocument.DoesNotExist:
            pass # Ya se registró el error de no encontrado
        return False, f"Error inesperado: {e}"

