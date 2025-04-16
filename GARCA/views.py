from django.shortcuts import render
from django.contrib import messages
from django.shortcuts import redirect, render
from GARCA.utils import create_backup
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

