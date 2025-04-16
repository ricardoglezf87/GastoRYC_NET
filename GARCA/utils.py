from urllib.parse import urlparse
import os
import datetime
import subprocess
import zipfile
import re
from django.conf import settings


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
    Crea una copia de seguridad de la base de datos usando dumpdata
    y opcionalmente la comprime.
    """
    backup_dir = os.path.join(settings.BASE_DIR, 'backups')
    os.makedirs(backup_dir, exist_ok=True)

    timestamp = datetime.datetime.now().strftime('%Y%m%d_%H%M%S')
    backup_filename = f'backup_{timestamp}.json'
    backup_filepath = os.path.join(backup_dir, backup_filename)
    zip_filepath = os.path.join(backup_dir, f'backup_{timestamp}.zip')

    command = [
        'python',
        'manage.py',
        'dumpdata',
        '--exclude=contenttypes',
        '--exclude=auth.permission',
        '--indent=2',
        '--output=' + backup_filepath
    ]

    try:
        print(f"Ejecutando comando: {' '.join(command)}")
        # Specify encoding for captured output/error for clearer logs
        process = subprocess.run(command, check=True, capture_output=True, text=True, encoding='utf-8', errors='surrogateescape')
        print("Salida de dumpdata:", process.stdout)
        if process.stderr:
             # Log potential warnings from dumpdata itself
            print("Advertencias/Errores de dumpdata:", process.stderr)

        print(f"Comprimiendo {backup_filepath} en {zip_filepath}")
        with zipfile.ZipFile(zip_filepath, 'w', zipfile.ZIP_DEFLATED) as zipf:
            zipf.write(backup_filepath, arcname=backup_filename)

        os.remove(backup_filepath)
        print(f"Archivo JSON original eliminado: {backup_filepath}")

        return zip_filepath

    except subprocess.CalledProcessError as e:
        print(f"Error durante la ejecución de dumpdata: {e}")
        # Decode stderr/stdout explicitly if possible for better error reporting
        stderr_output = e.stderr.decode('utf-8', errors='replace') if isinstance(e.stderr, bytes) else e.stderr
        stdout_output = e.stdout.decode('utf-8', errors='replace') if isinstance(e.stdout, bytes) else e.stdout
        print("Salida del proceso:", stdout_output)
        print("Error del proceso:", stderr_output)
        if os.path.exists(backup_filepath):
            try: os.remove(backup_filepath)
            except OSError: pass
        raise Exception(f"Error al ejecutar dumpdata: {stderr_output or stdout_output or str(e)}")
    except Exception as e:
        print(f"Error inesperado durante el backup: {e}")
        if os.path.exists(backup_filepath):
            try: os.remove(backup_filepath)
            except OSError: pass
        if os.path.exists(zip_filepath):
             try: os.remove(zip_filepath)
             except OSError: pass
        raise Exception(f"Error inesperado: {str(e)}")


def restore_backup(zip_filepath):
    """
    Restaura la base de datos desde un archivo .zip que contiene un .json
    con el formato esperado (backup_YYYYMMDD_HHMMSS.json).
    """
    backup_dir = os.path.dirname(zip_filepath)
    json_filename = None
    json_filepath = None
    backup_pattern = re.compile(r'^backup_\d{8}_\d{6}\.json$')
    extracted_dir = None # Keep track of extraction dir for cleanup

    try:
        print(f"Descomprimiendo archivo: {zip_filepath}")
        with zipfile.ZipFile(zip_filepath, 'r') as zipf:
            expected_json_files = [
                name for name in zipf.namelist()
                if backup_pattern.match(os.path.basename(name))
            ]

            if not expected_json_files:
                raise Exception("El archivo ZIP no contiene un archivo de backup válido (ej: backup_YYYYMMDD_HHMMSS.json).")
            if len(expected_json_files) > 1:
                 raise Exception(f"El archivo ZIP contiene múltiples archivos de backup válidos: {expected_json_files}. No se puede determinar cuál usar.")

            json_filename = expected_json_files[0]
            print(f"Archivo de backup encontrado en el ZIP: {json_filename}")

            # Extract to the backup directory
            zipf.extract(json_filename, path=backup_dir)
            json_filepath = os.path.join(backup_dir, json_filename)
            extracted_dir = os.path.dirname(json_filepath) # Store the actual dir path
            print(f"Archivo JSON extraído en: {json_filepath}")

        # --- Ejecutar loaddata ---
        command = [
            'python',
            'manage.py',
            'loaddata',
            json_filepath
        ]
        print(f"Ejecutando comando: {' '.join(command)}")
        # Specify encoding for captured output/error for clearer logs
        process = subprocess.run(command, check=True, capture_output=True, text=True, encoding='utf-8', errors='surrogateescape')
        print("Salida de loaddata:", process.stdout)
        if process.stderr:
            print("Advertencias/Errores de loaddata:", process.stderr)

    except subprocess.CalledProcessError as e:
        print(f"Error durante la ejecución de loaddata: {e}")
        stderr_output = e.stderr.decode('utf-8', errors='replace') if isinstance(e.stderr, bytes) else e.stderr
        stdout_output = e.stdout.decode('utf-8', errors='replace') if isinstance(e.stdout, bytes) else e.stdout
        print("Salida del proceso:", stdout_output)
        print("Error del proceso:", stderr_output)

        error_message = stderr_output or stdout_output or str(e)
        # Provide a more specific error message for UnicodeDecodeError
        if "UnicodeDecodeError" in error_message:
             detailed_error = (
                f"Error al ejecutar loaddata: Problema de codificación (UnicodeDecodeError). "
                f"El archivo JSON ('{os.path.basename(json_filepath or 'N/A')}') dentro del ZIP "
                f"probablemente no está en formato UTF-8 o contiene caracteres inválidos. "
                f"Verifique el archivo o los datos originales en la base de datos. Detalles: {error_message}"
            )
        else:
            detailed_error = f"Error al ejecutar loaddata: {error_message}"
        # Cleanup handled in finally block
        raise Exception(detailed_error) # Raise the more informative error

    except zipfile.BadZipFile:
        # Cleanup handled in finally block
        raise Exception("Error: El archivo subido no es un ZIP válido.")
    except Exception as e:
        print(f"Error inesperado durante la restauración: {e}")
        error_message = str(e)
        # Also check for UnicodeDecodeError in general exceptions
        if isinstance(e, UnicodeDecodeError) or "UnicodeDecodeError" in error_message:
             detailed_error = (
                f"Error inesperado: Problema de codificación (UnicodeDecodeError). "
                f"El archivo JSON ('{os.path.basename(json_filepath or 'N/A')}') dentro del ZIP "
                f"probablemente no está en formato UTF-8 o contiene caracteres inválidos. "
                f"Verifique el archivo o los datos originales en la base de datos. Detalles: {error_message}"
            )
        else:
             detailed_error = f"Error inesperado durante la restauración: {error_message}"
        # Cleanup handled in finally block
        raise Exception(detailed_error) # Raise the more informative error

    finally:
        # --- Limpieza del archivo JSON extraído ---
        if json_filepath and os.path.exists(json_filepath):
            try:
                os.remove(json_filepath)
                print(f"Archivo JSON temporal eliminado: {json_filepath}")
                # Opcional: intentar eliminar directorios vacíos creados durante la extracción
                # Use the stored extracted_dir path
                if extracted_dir and extracted_dir != backup_dir:
                    try:
                        # Check if directory is empty before removing
                        if not os.listdir(extracted_dir):
                            os.rmdir(extracted_dir)
                            print(f"Directorio temporal vacío eliminado: {extracted_dir}")
                        else:
                             print(f"Directorio temporal no eliminado (no está vacío): {extracted_dir}")
                    except OSError as e:
                        print(f"No se pudo eliminar el directorio temporal {extracted_dir}: {e}")
            except OSError as e:
                print(f"No se pudo eliminar el archivo JSON temporal {json_filepath}: {e}")

