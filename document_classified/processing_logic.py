# document_classified/processing_logic.py
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

from .models import documentInfo, documentType, ExtractedData


script_dir = os.path.dirname(os.path.abspath(__file__))
poppler_bin_path = os.path.join(script_dir, 'poppler', 'Library', 'bin')
pytesseract.pytesseract.tesseract_cmd = os.path.join(script_dir,'tesseract', 'tesseract.exe')

# ... (perform_ocr y identify_document_type sin cambios) ...
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
                config = '--psm 3 --oem 3'
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

def identify_document_type(text):
    """
    Intenta identificar el tipo de factura usando las reglas 'identifier' del JSON.
    """
    if not text:
        return None

    all_types = documentType.objects.filter(extraction_rules__isnull=False)

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


def extract_document_data(text, rules_or_type):
    """
    Extrae datos usando reglas (de un dict o un documentType) y parsea la fecha.
    """
    if not text:
        print("extract_document_data: Texto de entrada faltante.") # DEBUG
        return {}

    rules = None
    document_type_name = "Reglas directas" # Nombre por defecto para logs
    rules_source = "Desconocida" # Para saber de dónde vienen

    # Determinar si recibimos un diccionario de reglas o un objeto documentType
    if isinstance(rules_or_type, documentType):
        rules_source = f"documentType ID: {rules_or_type.id} ({rules_or_type.name})" # DEBUG
        if rules_or_type.extraction_rules:
            rules = rules_or_type.extraction_rules
            document_type_name = rules_or_type.name
        else:
            print(f"extract_document_data: {rules_source} no tiene reglas de extracción.") # DEBUG
            return {}
    elif isinstance(rules_or_type, dict):
        rules_source = "Diccionario directo (Probablemente desde Test Rules)" # DEBUG
        rules = rules_or_type
    else:
        print(f"extract_document_data: Entrada de reglas inválida (Tipo: {type(rules_or_type)}).") # DEBUG
        return {}

    # <<<--- NUEVO PRINT: Mostrar la fuente y las reglas que se usarán --->>>
    print(f"--- extract_document_data ---")
    print(f"Fuente de Reglas: {rules_source}")
    print(f"Reglas a Aplicar: {json.dumps(rules, indent=2)}") # Usar json.dumps para formato legible
    print(f"-----------------------------")
    # <<<----------------------------------------------------------------->>>

    if not rules:
        print("extract_document_data: Reglas de extracción vacías o no encontradas.") # DEBUG
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
                    print(f"Identificador encontrado ({document_type_name}, keyword '{rule_value}')")
                else:
                    # No añadir 'identifier_found': None si no se encuentra, es implícito
                    print(f"Identificador NO encontrado ({document_type_name}, keyword '{rule_value}')")
            # Añadir más lógica si hay otros tipos de reglas de identificador
        except Exception as e:
            print(f"Error aplicando regla de identificador ({document_type_name}): {e}")


    # --- Extracción de Fecha (con parseo) ---
    date_rule = rules.get('date')
    if date_rule:
        rule_type = date_rule.get('type')
        original_locale = None # Para restaurarlo después
        parsed_date = None # Inicializar fuera del try
        date_str = ""      # Inicializar fuera del try

        try:
            if rule_type == 'regex':
                pattern = date_rule.get('pattern')
                if pattern:
                    # <<<--- MODIFICACIÓN: Usar findall para fecha también? O quedarse con el primero? --->>>
                    # re.search encuentra la primera. Si puede haber varias fechas válidas y quieres
                    # una específica, podrías necesitar ajustar esto (ej. buscar cerca de una palabra clave).
                    # Por ahora, mantenemos re.search para la primera.
                    match = re.search(pattern, text, re.DOTALL | re.IGNORECASE)
                    if match:
                        # Asumir que la fecha está en el grupo 1 si la regex tiene paréntesis
                        # Si no, usar el match completo (grupo 0)
                        date_str = match.group(1).strip() if match.groups() else match.group(0).strip()
                        print(f"Cadena de fecha encontrada ({document_type_name}, regex '{pattern}'): '{date_str}'")

                        # 1. Intento inicial con parse_date (maneja formatos comunes ISO-like)
                        parsed_date = parse_date(date_str)

                        # <<<--- NUEVO: 2. Si parse_date falla, intentar formatos numéricos comunes explícitamente --->>>
                        if not parsed_date:
                            # --- INICIO DEBUG ---
                            print(f"DEBUG: parse_date falló. Intentando formatos numéricos comunes para '{date_str}' (repr: {repr(date_str)})")
                            # --- FIN DEBUG ---
                            common_numeric_formats = [
                                '%d/%m/%Y', # <-- El formato del log
                                '%d-%m-%Y',
                                '%d.%m.%Y',
                                '%d/%m/%y', # Formatos con año corto
                                '%d-%m-%y',
                                '%d.%m.%y',
                            ]
                            for fmt in common_numeric_formats:
                                # --- INICIO DEBUG ---
                                print(f"DEBUG: Intentando formato '{fmt}'...")
                                # --- FIN DEBUG ---
                                try:
                                    parsed_date = datetime.strptime(date_str, fmt).date()
                                    print(f"Fecha parseada (numérica común) con formato '{fmt}' usando '{date_str}'")
                                    break # Salir si se parsea
                                except ValueError as ve:
                                    # --- INICIO DEBUG ---
                                    print(f"DEBUG: Formato '{fmt}' falló para '{date_str}'. Error: {ve}")
                                    # --- FIN DEBUG ---
                                    continue
                                except Exception as e_inner: # Capturar otros errores
                                    # --- INICIO DEBUG ---
                                    print(f"DEBUG: Error inesperado intentando formato '{fmt}' para '{date_str}'. Error: {type(e_inner).__name__}: {e_inner}")
                                    # --- FIN DEBUG ---
                                    continue # Intentar siguiente formato
                        # <<<------------------------------------------------------------------------------------->>>

                        # 3. Si aún falla, intentar reemplazo de mes y formatos numéricos (si aplica)
                        if not parsed_date:
                            date_str_lower = date_str.lower()
                            date_str_numeric = date_str_lower
                            month_map = {'ene': 1, 'feb': 2, 'mar': 3, 'abr': 4, 'may': 5, 'jun': 6,
                                         'jul': 7, 'ago': 8, 'sep': 9, 'oct': 10, 'nov': 11, 'dic': 12}
                            replacement_done = False
                            for abbr, num in month_map.items():
                                # Buscar el patrón con guiones (más seguro que solo la abreviatura)
                                # Podrías hacerlo más robusto para buscar con / o . también si es necesario
                                pattern_to_replace = f"-{abbr}-"
                                if pattern_to_replace in date_str_numeric:
                                    replacement_value = f"-{num:02d}-" # Formatear con cero: 01, 02, ..., 10
                                    date_str_numeric = date_str_numeric.replace(pattern_to_replace, replacement_value)
                                    print(f"Mes reemplazado: '{abbr}' -> '{num:02d}'. Resultado: '{date_str_numeric}'")
                                    replacement_done = True
                                    break # Solo debe haber un mes

                            # Intentar parsear la versión numérica con formatos estándar si hubo reemplazo
                            if replacement_done:
                                # Usamos los mismos formatos comunes, pero con la cadena modificada
                                numeric_formats_after_replace = ['%d-%m-%Y', '%d/%m/%Y', '%d.%m.%Y', '%d-%m-%y', '%d/%m/%y', '%d.%m.%y']
                                for fmt in numeric_formats_after_replace:
                                    try:
                                        parsed_date = datetime.strptime(date_str_numeric, fmt).date()
                                        print(f"Fecha parseada (numérica post-reemplazo) con formato '{fmt}' usando '{date_str_numeric}'")
                                        break # Salir si se parsea
                                    except ValueError:
                                        continue

                        # 4. Si aún no se parseó (ni con parse_date, ni numérico común, ni con reemplazo numérico),
                        #    intentar con locale y formatos con nombre/abreviatura
                        if not parsed_date:
                            print(f"Parseo numérico/reemplazo falló o no aplicó. Intentando con locale para '{date_str.lower()}'...")
                            # --- Configurar locale ---
                            try:
                                original_locale = locale.getlocale(locale.LC_TIME)
                                locale.setlocale(locale.LC_TIME, 'es_ES.UTF-8')
                                print("Locale español (es_ES.UTF-8) establecido temporalmente.")
                            except locale.Error:
                                try:
                                    if not original_locale: original_locale = locale.getlocale(locale.LC_TIME)
                                    locale.setlocale(locale.LC_TIME, 'Spanish_Spain.1252')
                                    print("Locale español (Spanish_Spain.1252) establecido temporalmente.")
                                except locale.Error:
                                    print("Advertencia: No se pudo establecer locale español.")
                            # ----------------------------------------------------

                            # Formatos que dependen del locale o nombres/abreviaturas
                            # Asegúrate de que estos NO se solapen con los common_numeric_formats ya probados
                            locale_formats = [
                                '%d de %B de %Y', # "31 de julio de 2022"
                                '%d %B %Y',       # "31 julio 2022"
                                '%d %b %Y',       # "31 jul 2022"
                                '%d-%b-%Y',       # "31-jul-2022"
                                # Añadir más si es necesario
                            ]
                            for fmt in locale_formats:
                                try:
                                    # Usar date_str_lower aquí puede ser más robusto para nombres de meses
                                    parsed_date = datetime.strptime(date_str.lower(), fmt).date()
                                    print(f"Fecha parseada (locale) con formato '{fmt}' usando '{date_str.lower()}'")
                                    break
                                except ValueError:
                                    continue

                        # 5. Resultado final
                        if parsed_date:
                            extracted['document_date'] = parsed_date
                            print(f"Fecha parseada final ({document_type_name}): {parsed_date}")
                        else:
                            # Solo si todos los intentos fallaron
                            print(f"No se pudo parsear la fecha '{date_str}' ({document_type_name}) con ningún método.")
                    else:
                         print(f"Fecha NO encontrada ({document_type_name}, regex '{pattern}')") # DEBUG

        except Exception as e:
            print(f"Error aplicando regla de fecha ({document_type_name}): {e}")
            traceback.print_exc()
        finally:
            # --- Restaurar locale original ---
            if original_locale:
                try:
                    locale.setlocale(locale.LC_TIME, original_locale)
                    print("Locale original restaurado.")
                except locale.Error:
                     print("Advertencia: No se pudo restaurar el locale original.")

    # --- Extracción de Total ---
    total_rule = rules.get('total')
    if total_rule:
        rule_type = total_rule.get('type')
        try:
            if rule_type == 'regex':
                pattern = total_rule.get('pattern')
                if pattern:
                     # Usar findall para encontrar todas las ocurrencias
                     # <<<--- MODIFICACIÓN: Asegurarse que findall captura el grupo correcto --->>>
                     # Si el patrón tiene un grupo de captura (paréntesis), findall devuelve
                     # solo el contenido de ese grupo. Si no tiene grupo, devuelve la coincidencia completa.
                     # El patrón \b(\d+,\d{2})(?=€) tiene un grupo, así que findall devolverá ['19,60', '0,16'].
                     matches = re.findall(pattern, text, re.IGNORECASE | re.MULTILINE)
                     if matches:
                         potential_amounts = []
                         original_strings = {} # Para guardar el string original asociado a cada valor numérico

                         # <<<--- MODIFICACIÓN: Iterar sobre los strings encontrados por findall --->>>
                         for amount_str_raw in matches: # 'matches' ya contiene los strings ['19,60', '0,16']
                             # Limpiar cada match encontrado
                             amount_str_cleaned = amount_str_raw.replace(' ', '')
                             if ',' in amount_str_cleaned and '.' in amount_str_cleaned:
                                 if amount_str_cleaned.rfind('.') < amount_str_cleaned.rfind(','):
                                     amount_str_cleaned = amount_str_cleaned.replace('.', '')
                             amount_str_final = amount_str_cleaned.replace(',', '.')
                             amount_str_final = re.sub(r'[^\d.-]', '', amount_str_final)

                             try:
                                 # Convertir a float para comparación
                                 numeric_value = float(amount_str_final)
                                 potential_amounts.append(numeric_value)
                                 # Guardar el string original asociado a este valor (si no existe ya, para evitar duplicados si el valor se repite)
                                 if numeric_value not in original_strings:
                                     original_strings[numeric_value] = amount_str_raw
                             except ValueError:
                                 print(f"Advertencia: No se pudo convertir '{amount_str_final}' (desde '{amount_str_raw}') a número durante la búsqueda del mayor total.")
                                 continue # Saltar este match si no se puede convertir

                         if potential_amounts:
                             # Encontrar el mayor importe
                             largest_amount = max(potential_amounts)
                             extracted['total_amount'] = largest_amount # Guardar el mayor

                             # Obtener el string original que corresponde al mayor importe (para el log)
                             original_raw_string = original_strings.get(largest_amount, "N/A")

                             print(f"Total encontrado (mayor de {len(potential_amounts)} coincidencias con '{pattern}', raw '{original_raw_string}'): {extracted['total_amount']}")
                         else:
                             print(f"Total NO encontrado ({document_type_name}, regex '{pattern}' - ninguna coincidencia pudo convertirse a número)")

                     else:
                        print(f"Total NO encontrado ({document_type_name}, regex '{pattern}')")

        except Exception as e:
            print(f"Error aplicando regla de total ({document_type_name}): {e}")
            traceback.print_exc()


    # ... (Extraer otros campos si los tienes) ...

    print(f"Datos extraídos finales ({document_type_name}): {extracted}") # DEBUG
    return extracted

# ... (process_document_document sin cambios) ...
def process_document_document(document_id):
    """
    Función principal que orquesta el procesamiento de un documentInfo.
    Ahora reutiliza el texto OCR existente si está disponible.
    """
    try:
        # Obtener el documento y marcar como 'PROCESSING'
        doc = documentInfo.objects.get(id=document_id)
        print(f"Procesando/Reprocesando Documento ID: {doc.id}, Archivo: {doc.file.name}")
        doc.status = 'PROCESSING'
        # Guardar el estado 'PROCESSING' inmediatamente
        doc.save(update_fields=['status'])

        extracted_text = None

        # --- INICIO MODIFICACIÓN: Reutilizar OCR existente ---
        if doc.extracted_text and doc.extracted_text.strip():
            print(f"Reprocesando: Usando texto OCR existente para Doc ID: {doc.id}")
            extracted_text = doc.extracted_text
        else:
            # Si no hay texto previo (o está vacío), intentar OCR
            print(f"Procesando/Reprocesando: Realizando OCR para Doc ID: {doc.id} (no hay texto previo o está vacío)")
            # Asegurarse que el archivo existe antes de intentar OCR
            if not doc.file or not hasattr(doc.file, 'path') or not os.path.exists(doc.file.path):
                 error_msg = "Archivo no encontrado o inaccesible para OCR"
                 print(f"{error_msg} para Documento ID: {doc.id}")
                 doc.status = 'FAILED'
                 doc.extracted_text = error_msg # Guardar el error específico
                 doc.save(update_fields=['status', 'extracted_text'])
                 return False, error_msg # Devolver el error específico

            extracted_text = perform_ocr(doc.file.path)

            if extracted_text is None:
                # Si el OCR falla ahora
                error_msg = "Error durante OCR"
                print(f"Fallo OCR para Documento ID: {doc.id}")
                doc.status = 'FAILED'
                doc.extracted_text = error_msg # Guardar el error específico
                doc.save(update_fields=['status', 'extracted_text'])
                return False, error_msg # Devolver el error específico
            else:
                # Si el OCR fue exitoso ahora, guardar el texto nuevo
                doc.extracted_text = extracted_text
                # No guardamos aquí todavía, se guarda al final con el estado
        # --- FIN MODIFICACIÓN ---

        # --- Lógica de Identificación y Extracción (sin cambios) ---
        # Ahora SIEMPRE tenemos algo en 'extracted_text' si llegamos aquí
        # (ya sea el texto previo o el recién extraído)

        document_type = identify_document_type(extracted_text)
        if document_type:
            doc.document_type = document_type
            data = extract_document_data(extracted_text, document_type)
            if data.get('document_date') or data.get('total_amount'):
                ExtractedData.objects.update_or_create(
                    document=doc,
                    defaults={
                        'document_date': data.get('document_date'),
                        'total_amount': data.get('total_amount'),
                    }
                )
                doc.status = 'PROCESSED'
                print(f"Datos extraídos para Documento ID: {doc.id}")
            else:
                doc.status = 'NEEDS_MAPPING'
                print(f"Tipo identificado pero sin datos extraídos para Documento ID: {doc.id}")
        else:
            doc.status = 'NEEDS_MAPPING'
            print(f"Documento ID: {doc.id} necesita mapeo manual.")

        # Guardar el estado final y el texto (si se acaba de extraer)
        doc.save() # Guarda todos los campos modificados (status, document_type, extracted_text si cambió)
        print(f"Procesamiento finalizado para Documento ID: {doc.id}. Estado: {doc.status}")
        return True, doc.status

    except documentInfo.DoesNotExist: # Manejo específico si el ID no existe
        print(f"Error: Documento con ID {document_id} no encontrado.")
        # No hay objeto 'doc' para guardar aquí
        return False, "Documento no encontrado"
    except Exception as e:
        print(f"Error procesando documento {document_id}: {e}")
        traceback.print_exc() # Imprime el traceback completo en la consola del servidor
        try:
            # Intenta marcar como FAILED si el objeto 'doc' existe
            doc = documentInfo.objects.get(id=document_id)
            doc.status = 'FAILED'
            # Guardar un mensaje de error más genérico si no es el de OCR
            if not doc.extracted_text or "Error durante OCR" not in doc.extracted_text:
                 doc.extracted_text = f"Error inesperado: {e}"
            doc.save(update_fields=['status', 'extracted_text'])
        except documentInfo.DoesNotExist:
            pass # El documento no existe, no hay nada que guardar
        except Exception as save_err:
             print(f"Error adicional al intentar guardar estado FAILED para {document_id}: {save_err}")
        return False, f"Error inesperado: {e}"
