import gspread
from django.conf import settings
import logging

logger = logging.getLogger(__name__)

def get_gspread_client():
    """Autentica y devuelve un cliente gspread."""
    try:
        return gspread.service_account(filename=settings.GOOGLE_CREDENTIALS_FILE_PATH)
    except Exception as e:
        logger.error(f"Error al autenticar con Google Service Account: {e}", exc_info=True)
        raise  # Relanzar para que la vista lo maneje

def update_google_sheet_with_data(data_for_sheet):
    """
    Actualiza una Google Sheet específica con los datos proporcionados.
    Crea el archivo o la pestaña si no existen.
    """
    try:
        gc = get_gspread_client()
        folder_id = settings.GOOGLE_DRIVE_BIARCA_FOLDER_ID
        sheet_name = settings.GOOGLE_SHEET_NAME
        sheet_id = getattr(settings, 'GOOGLE_SHEET_ID', None) # Obtener el ID si está definido
        worksheet_name = settings.GOOGLE_SHEET_WORKSHEET_NAME

        # 1. Buscar o crear la hoja de cálculo
        spreadsheet = None
        try:
            # Intenta abrir la hoja de cálculo por su nombre.
            # gspread.open() puede encontrarla si está compartida con la cuenta de servicio
            # o si la cuenta de servicio tiene acceso amplio.            
            if sheet_id:
                try:
                    spreadsheet = gc.open_by_key(sheet_id)
                    logger.info(f"Hoja de cálculo '{sheet_name}' (ID: {sheet_id}) abierta por ID.")
                except gspread.exceptions.SpreadsheetNotFound:
                    logger.warning(f"Hoja de cálculo con ID '{sheet_id}' no encontrada. Intentando crear una nueva con nombre '{sheet_name}' en la carpeta ID: {folder_id}...")
                    # Si el ID no funciona, es mejor crearla con el nombre y folder_id para asegurar la ubicación
                    spreadsheet = gc.create(sheet_name, folder_id=folder_id)
                    logger.info(f"Hoja de cálculo '{sheet_name}' creada con ID: {spreadsheet.id} en la carpeta especificada. Actualiza GOOGLE_SHEET_ID en settings.py con este nuevo ID: {spreadsheet.id}")
            else: # Si no hay ID en settings, proceder como antes
                spreadsheet = gc.open(sheet_name)
                logger.info(f"Hoja de cálculo '{sheet_name}' encontrada por nombre.")
        except gspread.exceptions.SpreadsheetNotFound:
            logger.info(f"Hoja de cálculo '{sheet_name}' no encontrada por nombre. Creando una nueva en la carpeta ID: {folder_id}...")
            spreadsheet = gc.create(sheet_name, folder_id=folder_id)
            logger.info(f"Hoja de cálculo '{sheet_name}' creada con ID: {spreadsheet.id}")
            if not sheet_id: # Solo sugerir si no se partió de un ID
                logger.info(f"Considera añadir GOOGLE_SHEET_ID = '{spreadsheet.id}' a tus settings.py para futuras aperturas directas.")
        
        # 2. Obtener o crear la pestaña (worksheet) - esta parte no cambia
        try:
            worksheet = spreadsheet.worksheet(worksheet_name)
            logger.info(f"Pestaña '{worksheet_name}' encontrada.")
        except gspread.exceptions.WorksheetNotFound:
            logger.info(f"Pestaña '{worksheet_name}' no encontrada. Creando nueva pestaña...")
            worksheet = spreadsheet.add_worksheet(title=worksheet_name, rows="1", cols="1") # Iniciar pequeña

        # 3. Limpiar la pestaña y escribir los nuevos datos
        worksheet.clear()
        worksheet.update('A1', data_for_sheet, value_input_option='USER_ENTERED')
        logger.info(f"Hoja de cálculo '{sheet_name}' (pestaña '{worksheet_name}') actualizada con {len(data_for_sheet)-1} filas de datos.")
        return True, f"Google Sheet '{sheet_name}' actualizada exitosamente."

    except Exception as e:
        logger.error(f"Error al actualizar Google Sheet: {e}", exc_info=True)
        return False, f"Error al actualizar Google Sheet: {e}"