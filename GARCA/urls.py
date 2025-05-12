from django.contrib import admin
from django.urls import path, include
from django.conf import settings
from django.conf.urls.static import static
from .views import index

urlpatterns = [
    path('admin/', admin.site.urls),
    path('', index, name='index'),
    path('accounts/', include('accounts.urls')),
    path('entries/', include('entries.urls')),
    path('transactions/', include('transactions.urls')),
    path('attachments/', include('attachments.urls')),
    path('bank_imports/', include('bank_imports.urls')),
    path('reports/', include('reports.urls')),  
    path('async/', include('async_tasks.urls')),
    path('api/', include('document_classified.urls')),
    
] + static(settings.MEDIA_URL, document_root=settings.MEDIA_ROOT)
