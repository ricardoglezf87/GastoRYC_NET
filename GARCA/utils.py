from urllib.parse import urlparse
import os
import datetime
import subprocess
import zipfile
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
    # Define dónde guardar las copias de seguridad
    backup_dir = os.path.join(settings.BASE_DIR, 'backups')
    os.makedirs(backup_dir, exist_ok=True) # Crea el directorio si no existe

    # Nombre del archivo de backup con fecha y hora
    timestamp = datetime.datetime.now().strftime('%Y%m%d_%H%M%S')
    backup_filename = f'backup_{timestamp}.json'
    backup_filepath = os.path.join(backup_dir, backup_filename)
    zip_filepath = os.path.join(backup_dir, f'backup_{timestamp}.zip')

    # Comando dumpdata (excluye contenttypes y auth.permission para evitar problemas comunes)
    # Puedes ajustar las apps a incluir o excluir según necesites
    command = [
        'python',
        'manage.py',
        'dumpdata',
        '--exclude=contenttypes',
        '--exclude=auth.permission',
        '--indent=2', # Para que sea legible (opcional)
        '--output=' + backup_filepath
    ]

    try:
        # Ejecuta el comando dumpdata
        print(f"Ejecutando comando: {' '.join(command)}") # Log para depuración
        process = subprocess.run(command, check=True, capture_output=True, text=True)
        print("Salida de dumpdata:", process.stdout)
        print("Errores de dumpdata:", process.stderr)


        # --- Opcional: Comprimir el archivo ---
        print(f"Comprimiendo {backup_filepath} en {zip_filepath}")
        with zipfile.ZipFile(zip_filepath, 'w', zipfile.ZIP_DEFLATED) as zipf:
            zipf.write(backup_filepath, arcname=backup_filename)

        # Eliminar el archivo JSON original después de comprimir
        os.remove(backup_filepath)
        print(f"Archivo JSON original eliminado: {backup_filepath}")

        return zip_filepath # Devuelve la ruta del archivo zip
        # --- Fin Opcional ---

        # Si no comprimes, devuelve la ruta del archivo JSON
        # return backup_filepath

    except subprocess.CalledProcessError as e:
        print(f"Error durante la ejecución de dumpdata: {e}")
        print("Salida del proceso:", e.stdout)
        print("Error del proceso:", e.stderr)
        # Intenta eliminar el archivo parcial si existe y falló
        if os.path.exists(backup_filepath):
            try:
                os.remove(backup_filepath)
            except OSError:
                pass # Ignora si no se puede eliminar
        raise Exception(f"Error al ejecutar dumpdata: {e.stderr or e.stdout or str(e)}")
    except Exception as e:
        print(f"Error inesperado durante el backup: {e}")
         # Intenta eliminar el archivo parcial si existe y falló
        if os.path.exists(backup_filepath):
            try:
                os.remove(backup_filepath)
            except OSError:
                pass
        if os.path.exists(zip_filepath):
             try:
                os.remove(zip_filepath)
             except OSError:
                pass
        raise Exception(f"Error inesperado: {str(e)}")