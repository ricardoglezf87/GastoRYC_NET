import logging
from datetime import datetime,timedelta
from googleapiclient.discovery import build
from google.oauth2.service_account import Credentials # Asumiendo que la usas
from django.conf import settings # Para credenciales y IDs de hoja
from django.core.cache import cache

logger = logging.getLogger(__name__)

# --- Configuración ---
# Estas variables se cargan desde settings.py y son cruciales para la conexión con Google Sheets.
GOOGLE_SHEET_ID = settings.GOOGLE_SHEET_ID # Reemplaza con tu ID de Spreadsheet
GOOGLE_SHEET_NAME = settings.GOOGLE_SHEET_WORKSHEET_NAME # Reemplaza con el nombre de tu hoja
GOOGLE_SERVICE_ACCOUNT_FILE = settings.GOOGLE_CREDENTIALS_FILE_PATH # Reemplaza
DATE_COLUMN_INDEX_FOR_FILTER = 1 # Índice (0-based) de la columna de fecha usada para filtrar al borrar por periodo.

# Definir la cabecera estándar para la hoja. Debe coincidir con los datos que se envían desde la vista.
SHEET_HEADERS = [
    'id', 'date', 'description', 'transaction_id', 'accountId',
    'account', 'debit', 'credit', 'parent_id', 'closed'
]


SCOPES = ['https://www.googleapis.com/auth/spreadsheets']

def get_google_sheets_service():
    """Autentica y construye el servicio de Google Sheets."""
    if not GOOGLE_SERVICE_ACCOUNT_FILE:
        raise ValueError("La ruta al archivo de cuenta de servicio de Google no está configurada.")
    creds = Credentials.from_service_account_file(GOOGLE_SERVICE_ACCOUNT_FILE, scopes=SCOPES)
    service = build('sheets', 'v4', credentials=creds)
    return service

def get_sheet_id_by_name(service, spreadsheet_id, sheet_name):
    """Obtiene el ID numérico de una hoja dado su nombre."""
    try:
        sheet_metadata = service.spreadsheets().get(spreadsheetId=spreadsheet_id).execute()
        sheets = sheet_metadata.get('sheets', '')
        for sheet in sheets:
            if sheet.get('properties', {}).get('title', '') == sheet_name:
                return sheet.get('properties', {}).get('sheetId', 0)
        raise ValueError(f"No se encontró la hoja con nombre '{sheet_name}' en el spreadsheet '{spreadsheet_id}'.")
    except Exception as e:
        logger.error(f"Error obteniendo sheetId para '{sheet_name}': {e}", exc_info=True)
        raise

def update_google_sheet_with_data(data_to_upload, task_id, start_date=None, end_date=None):
    """
    Actualiza la hoja de Google Sheets.
    Si start_date y end_date se proporcionan, borra las filas de ese periodo y luego añade los nuevos datos.
    Si no, limpia toda la hoja (A1:Z), escribe cabeceras y luego los nuevos datos.
    """
    if not GOOGLE_SHEET_ID or not GOOGLE_SHEET_NAME:
        logger.error(f"Task {task_id}: GOOGLE_SHEET_ID o GOOGLE_SHEET_NAME no están configurados.")
        raise ValueError("Configuración de Google Sheet incompleta.")

    sheets_service = get_google_sheets_service()
    spreadsheet_id = GOOGLE_SHEET_ID
    sheet_name = GOOGLE_SHEET_NAME
    
    processed_count_for_cache = 0
    total_count_for_cache = len(data_to_upload) # data_to_upload ahora son solo filas de datos

    try:
        # --- Parte 1: Borrar datos existentes ---
        if start_date and end_date:
            logger.info(f"Task {task_id}: Borrando datos existentes en Google Sheet para el periodo {start_date} a {end_date}")
            cache.set(f'sheet_update_progress_{task_id}', {
                'processed': 0, 'total': total_count_for_cache, 'status': 'processing',
                'stage': 'deleting_old_data', 
                'message': f'Eliminando datos del periodo {start_date.strftime("%Y-%m-%d")} al {end_date.strftime("%Y-%m-%d")}...'
            }, timeout=3600)

            numeric_sheet_id = get_sheet_id_by_name(sheets_service, spreadsheet_id, sheet_name)
            header_rows = 1 # Asumimos que la primera fila es cabecera y no se borra al filtrar por periodo.
            # Ajusta el rango si tus datos son más anchos o si la columna de fecha no es la B (índice 1)
            # Aquí leemos solo la columna de fecha para eficiencia, asumiendo que es la columna B (DATE_COLUMN_INDEX_FOR_FILTER + 1 en A1 notation)
            date_column_letter = chr(ord('A') + DATE_COLUMN_INDEX_FOR_FILTER)
            range_to_read_dates = f'{sheet_name}!{date_column_letter}{header_rows + 1}:{date_column_letter}'
            
            result = sheets_service.spreadsheets().values().get(
                spreadsheetId=spreadsheet_id,
                range=range_to_read_dates,
                valueRenderOption='UNFORMATTED_VALUE', # Obtiene el valor subyacente (ej. número de serie para fechas).
                dateTimeRenderOption='SERIAL_NUMBER'  # Asegura que las fechas se devuelvan como números de serie.
            ).execute()
            sheet_date_values = result.get('values', [])

            requests_for_delete = []
            if sheet_date_values:
                for i, row_cells in enumerate(sheet_date_values):
                    if not row_cells: continue # Fila vacía en la columna de fecha
                    date_val = row_cells[0] # Puede ser un número de serie o un string.
                    current_row_index_in_sheet = i + header_rows + 1 # Índice real en la hoja (base 1)
                    try:
                        if isinstance(date_val, (int, float)):
                            # Convertir número de serie de Excel/Google Sheets a fecha de Python.
                            # Google Sheets (como Excel) usa el epoch de 1899-12-30 para los números de serie.
                            # El día 1 corresponde a 1899-12-31.
                            excel_epoch = datetime(1899, 12, 30)
                            row_date = (excel_epoch + timedelta(days=date_val)).date()
                        elif isinstance(date_val, str):
                            # Fallback: intentar parsear como string si no es un número de serie.
                            # Esto podría ser necesario si la celda no fue interpretada como fecha por Sheets
                            # y se almacenó como un string literal, o si valueRenderOption no devolvió un número.
                            row_date = datetime.strptime(date_val, '%Y-%m-%d').date()
                        else:
                            logger.warning(f"Task {task_id}: Tipo de valor de fecha no esperado '{type(date_val)}' en la fila {current_row_index_in_sheet} de la hoja. Se omite para borrado.")
                            continue

                        if start_date <= row_date <= end_date:
                            requests_for_delete.append({
                                'deleteDimension': {
                                    'range': {
                                        'sheetId': numeric_sheet_id,
                                        'dimension': 'ROWS',
                                        'startIndex': current_row_index_in_sheet - 1, # API usa índices base 0.
                                        'endIndex': current_row_index_in_sheet
                                    }
                                }
                            })
                    except Exception as e_parse: # Captura más general para errores de conversión o parseo
                        logger.warning(f"Task {task_id}: No se pudo convertir/parsear el valor de fecha '{date_val}' (tipo: {type(date_val)}) en la fila {current_row_index_in_sheet} de la hoja: {e_parse}. Se omite para borrado.")
                        continue
            
            if requests_for_delete:
                # La API de Google Sheets procesa las eliminaciones de forma más eficiente de abajo hacia arriba.
                requests_for_delete.sort(key=lambda r: r['deleteDimension']['range']['startIndex'], reverse=True)
                
                logger.info(f"Task {task_id}: Se eliminarán {len(requests_for_delete)} filas del periodo especificado.")
                body = {'requests': requests_for_delete}
                sheets_service.spreadsheets().batchUpdate(spreadsheetId=spreadsheet_id, body=body).execute()
                logger.info(f"Task {task_id}: Filas del periodo eliminadas exitosamente.")
            else:
                logger.info(f"Task {task_id}: No se encontraron filas para eliminar en el periodo especificado.")
        
        else: # Si no hay rango de fechas, se borra toda la hoja (A1:Z).
            logger.info(f"Task {task_id}: Borrando todos los datos (A1:Z) en Google Sheet '{sheet_name}'.")
            cache.set(f'sheet_update_progress_{task_id}', {
                'processed': 0, 'total': total_count_for_cache, 'status': 'processing',
                'stage': 'clearing_sheet', 'message': 'Limpiando hoja completa...'
            }, timeout=3600)
            
            # Borra todo el contenido de la hoja, incluyendo cabeceras previas
            clear_range = f'{sheet_name}!A1:Z' # Ajustar 'Z' si se necesitan más columnas.
            sheets_service.spreadsheets().values().clear(
                spreadsheetId=spreadsheet_id, range=clear_range, body={}
            ).execute()
            logger.info(f"Task {task_id}: Datos (desde fila 2) eliminados de la hoja '{sheet_name}'.")

        # --- Parte 2: Escribir los nuevos datos ---
        cache.set(f'sheet_update_progress_{task_id}', {
            'processed': 0, 'total': total_count_for_cache, 'status': 'processing',
            'stage': 'uploading_data', 'message': 'Subiendo nuevos datos...'
        }, timeout=3600)

        if not data_to_upload:
            logger.info(f"Task {task_id}: No hay datos nuevos para subir al Google Sheet.")
            final_message = 'No hay datos nuevos para el periodo seleccionado.' if start_date else 'No hay datos nuevos para subir.'
            cache.set(f'sheet_update_progress_{task_id}', {
                'processed': 0, 'total': 0, 'status': 'completed', 'stage': 'finished',
                'message': final_message
            }, timeout=3600)
            return {'status': 'completed', 'message': final_message, 'processed': 0, 'total': 0}

        if not (start_date and end_date): # Si se limpió toda la hoja (sin filtro de periodo).
            # Escribir cabeceras en la primera fila
            sheets_service.spreadsheets().values().update(
                spreadsheetId=spreadsheet_id,
                range=f'{sheet_name}!A1',
                valueInputOption='USER_ENTERED',
                body={'values': [SHEET_HEADERS]} # Usar la cabecera estándar definida
            ).execute()
            logger.info(f"Task {task_id}: Cabeceras escritas en '{sheet_name}'.")
            
            # Escribir datos desde la segunda fila
            target_range_for_data = f'{sheet_name}!A2'
            body_data = {'values': data_to_upload} # data_to_upload ahora son solo filas de datos
            sheets_service.spreadsheets().values().update(
                spreadsheetId=spreadsheet_id,
                range=target_range_for_data,
                valueInputOption='USER_ENTERED',
                body=body_data
            ).execute()
            logger.info(f"Task {task_id}: {len(data_to_upload)} filas de datos escritas (update) desde A2 en '{sheet_name}'.")

        else: # Si hay start_date y end_date, se borró selectivamente, ahora se AÑADEN (append) los datos.
            # data_to_upload ya son solo las filas de datos para el periodo
            append_body = {'values': data_to_upload} 
            sheets_service.spreadsheets().values().append(
                spreadsheetId=spreadsheet_id,
                range=f'{sheet_name}!A1', # Append buscará la primera fila vacía después de la tabla existente
                valueInputOption='USER_ENTERED',
                insertDataOption='INSERT_ROWS',
                body=append_body
            ).execute()
            logger.info(f"Task {task_id}: {len(data_to_upload)} filas de datos añadidas (append) a '{sheet_name}' para el periodo.")

        processed_count_for_cache = len(data_to_upload)
        final_message = f'Actualización completada. {processed_count_for_cache} registros procesados.'
        cache.set(f'sheet_update_progress_{task_id}', {
            'processed': processed_count_for_cache, 'total': total_count_for_cache,
            'status': 'completed', 'stage': 'finished', 'message': final_message
        }, timeout=3600)
        
        return {'status': 'completed', 'message': final_message, 'processed': processed_count_for_cache, 'total': total_count_for_cache}

    except Exception as e:
        logger.error(f"Task {task_id}: Error al actualizar Google Sheet: {e}", exc_info=True)
        error_message = f'Error al actualizar Google Sheet: {str(e)}'
        current_progress = cache.get(f'sheet_update_progress_{task_id}', {})
        cache.set(f'sheet_update_progress_{task_id}', {
            'processed': current_progress.get('processed', 0),
            'total': current_progress.get('total', total_count_for_cache),
            'status': 'error', 'stage': current_progress.get('stage', 'error'),
            'message': error_message
        }, timeout=3600)
        raise ValueError(error_message) from e
