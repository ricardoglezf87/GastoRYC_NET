"""
URL configuration for GARCA project.

The `urlpatterns` list routes URLs to views. For more information please see:
    https://docs.djangoproject.com/en/5.1/topics/http/urls/
Examples:
Function views
    1. Add an import:  from my_app import views
    2. Add a URL to urlpatterns:  path('', views.home, name='home')
Class-based views
    1. Add an import:  from other_app.views import Home
    2. Add a URL to urlpatterns:  path('', Home.as_view(), name='home')
Including another URLconf
    1. Import the include() function: from django.urls import include, path
    2. Add a URL to urlpatterns:  path('blog/', include('blog.urls'))
"""
from django.contrib import admin
from django.urls import path, include
from django.conf import settings
from django.conf.urls.static import static
from .views import create_backup_view, index, restore_backup_view  # Importar la función index desde views.py

urlpatterns = [
    path('admin/', admin.site.urls),
    path('', index, name='index'),  # Usar la función index importada
    path('accounts/', include('accounts.urls')),
    path('entries/', include('entries.urls')),
    path('transactions/', include('transactions.urls')),
    path('attachments/', include('attachments.urls')),
    path('bank_imports/', include('bank_imports.urls')),
    path('reports/', include('reports.urls')),  # Añadir esta línea
    path('async/', include('async_tasks.urls')),
    path('backup/create/', create_backup_view, name='create_backup'),
    path('backup/restore/', restore_backup_view, name='restore_backup'),
    
] + static(settings.MEDIA_URL, document_root=settings.MEDIA_ROOT)
