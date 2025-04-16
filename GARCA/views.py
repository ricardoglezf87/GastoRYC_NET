from django.shortcuts import render
from django.contrib import messages
from django.shortcuts import redirect, render
from GARCA.utils import create_backup, restore_backup
from django.core.files.storage import FileSystemStorage
from django.conf import settings
import os

def index(request):
    return render(request, 'admin/index.html')



def create_backup_view(request):
    try:
        backup_file_path = create_backup()
        messages.success(request, f"Copia de seguridad creada exitosamente en: {backup_file_path}")
    except Exception as e:
        messages.error(request, f"Error al crear la copia de seguridad: {e}")

    # Redirige a alguna página, por ejemplo, la página principal del admin o la anterior
    # return redirect('admin:index')
    # O si quieres redirigir a la página anterior:
    referer_url = request.META.get('HTTP_REFERER', '/')
    return redirect(referer_url)

def restore_backup_view(request):
    if request.method == 'POST' and request.FILES.get('backup_file'):
        backup_file = request.FILES['backup_file']

        # Validar que sea un archivo .zip (puedes añadir más validaciones)
        if not backup_file.name.endswith('.zip'):
            messages.error(request, "Error: El archivo debe ser un .zip.")
            return render(request, 'admin/restore_backup.html')

        # Guardar temporalmente el archivo subido
        fs = FileSystemStorage(location=os.path.join(settings.BASE_DIR, 'temp_backups'))
        # Crear directorio temporal si no existe
        os.makedirs(fs.location, exist_ok=True)
        filename = fs.save(backup_file.name, backup_file)
        uploaded_file_path = fs.path(filename)

        try:
            # Llamar a la función de restauración en utils.py
            restore_backup(uploaded_file_path)
            messages.success(request, "Restauración completada exitosamente desde el archivo.")
            # Limpiar el archivo temporal después de usarlo
            fs.delete(filename)
            # Redirigir a una página segura, como el inicio del admin
            return redirect('admin:index')
        except Exception as e:
            messages.error(request, f"Error durante la restauración: {e}")
            # Intentar limpiar el archivo temporal incluso si hay error
            try:
                fs.delete(filename)
            except Exception as delete_error:
                messages.warning(request, f"No se pudo eliminar el archivo temporal: {delete_error}")
            # Permanecer en la página de restauración para mostrar el error
            return render(request, 'admin/restore_backup.html')

    # Si es GET o no hay archivo en POST, mostrar el formulario
    return render(request, 'admin/restore_backup.html')