from urllib.parse import urlparse
import os
import datetime
import subprocess
import zipfile
import re
from django.conf import settings
from django.core.exceptions import ImproperlyConfigured 


def add_breadcrumb(request, text, url, max_length=10):
    breadcrumbs = request.session.get('breadcrumbs', [])
    path_without_query = urlparse(url).path
    new_breadcrumb = (text, path_without_query)
    breadcrumb_set = {f"{text}-{url}" for text, url in breadcrumbs}
    new_breadcrumb_str = f"{new_breadcrumb[0]}-{new_breadcrumb[1]}"

    if new_breadcrumb_str not in breadcrumb_set:
        breadcrumbs.append(new_breadcrumb)

    # Find the index of the new breadcrumb
    new_index = -1
    for i, (t, u) in enumerate(breadcrumbs):
        if f"{t}-{u}" == new_breadcrumb_str:
            new_index = i
            break

    if new_index != -1:
        # Remove breadcrumbs after the new index
        breadcrumbs = breadcrumbs[:new_index + 1]

    # Trim breadcrumbs if the length exceeds max_length
    if len(breadcrumbs) > max_length:
        breadcrumbs = breadcrumbs[:max_length]

    request.session['breadcrumbs'] = breadcrumbs

def clear_breadcrumbs(request):
    request.session['breadcrumbs'] = []

def remove_breadcrumb(request, text, url):
    breadcrumbs = request.session.get('breadcrumbs', [])
    path_without_query = urlparse(url).path
    breadcrumb_to_remove = (text, path_without_query)
    breadcrumb_set = {f"{text}-{url}" for text, url in breadcrumbs}
    breadcrumb_to_remove_str = f"{breadcrumb_to_remove[0]}-{breadcrumb_to_remove[1]}"

    new_breadcrumbs = [bc for bc in breadcrumbs if f"{bc[0]}-{bc[1]}" != breadcrumb_to_remove_str]
    request.session['breadcrumbs'] = new_breadcrumbs

def go_back_breadcrumb(request):
    breadcrumbs = request.session.get('breadcrumbs', [])
    if len(breadcrumbs) > 1:
        breadcrumbs.pop()  # Remove the last breadcrumb
    request.session['breadcrumbs'] = breadcrumbs

def get_breadcrumbs(request):
    return request.session.get('breadcrumbs', [])

def create_backup():
    """
    Crea una copia de seguridad copiando directamente el archivo de base de datos SQLite
    y comprimiéndolo en un archivo .zip.
    """
    # --- Validación de la configuración de la base de datos ---
    db_config = settings.DATABASES.get('default')
    if not db_config or db_config.get('ENGINE') != 'django.db.backends.sqlite3':
        raise ImproperlyConfigured("La configuración de la base de datos 'default' no es SQLite3 o no está definida.")

    db_path = db_config.get('NAME')
    if not db_path:
        raise ImproperlyConfigured("La ruta del archivo de base de datos SQLite ('NAME') no está definida en settings.DATABASES['default'].")

    if not os.path.exists(db_path):
        raise FileNotFoundError(f"El archivo de base de datos SQLite no se encontró en: {db_path}")

    # --- Preparación de directorios y nombres de archivo ---
    backup_dir = os.path.join(settings.BASE_DIR, 'backups')
    os.makedirs(backup_dir, exist_ok=True)

    timestamp = datetime.datetime.now().strftime('%Y%m%d_%H%M%S')
    # Nombre del archivo SQLite dentro del ZIP
    db_filename_in_zip = f'db_backup_{timestamp}.sqlite3'
    # Nombre del archivo ZIP final
    zip_filename = f'backup_sqlite_{timestamp}.zip'
    zip_filepath = os.path.join(backup_dir, zip_filename)

    try:
        print(f"Iniciando copia de seguridad de archivo SQLite: {db_path}")
        print(f"Creando archivo ZIP: {zip_filepath}")

        # --- Crear el archivo ZIP y añadir el archivo de base de datos ---
        # Copiar directamente el archivo de la base de datos al ZIP
        # Usar shutil.copy2 podría ser una opción si necesitas copiarlo primero,
        # pero añadirlo directamente al zip es más eficiente.
        # ¡Importante! Esto copia el estado actual del archivo. Si hay transacciones
        # en curso, la copia podría no ser perfectamente consistente. Para mayor
        # seguridad, se debería bloquear la base de datos o detener la aplicación,
        # pero eso no es práctico desde aquí.
        with zipfile.ZipFile(zip_filepath, 'w', zipfile.ZIP_DEFLATED) as zipf:
            zipf.write(db_path, arcname=db_filename_in_zip)
            print(f"Archivo '{os.path.basename(db_path)}' añadido al ZIP como '{db_filename_in_zip}'")

        print(f"Copia de seguridad SQLite completada exitosamente en: {zip_filepath}")
        return zip_filepath

    except FileNotFoundError as e: # Captura específica por si acaso
        print(f"Error: {e}")
        raise Exception(f"Error de archivo no encontrado durante el backup: {e}")
    except zipfile.BadZipFile as e:
        print(f"Error al crear el archivo ZIP: {e}")
        # Intentar limpiar el ZIP si se creó parcialmente
        if os.path.exists(zip_filepath):
            try: os.remove(zip_filepath)
            except OSError: pass
        raise Exception(f"Error al crear el archivo ZIP: {e}")
    except Exception as e:
        print(f"Error inesperado durante el backup de SQLite: {e}")
        # Intentar limpiar el ZIP si se creó
        if os.path.exists(zip_filepath):
            try: os.remove(zip_filepath)
            except OSError: pass
        raise Exception(f"Error inesperado durante el backup: {str(e)}")




