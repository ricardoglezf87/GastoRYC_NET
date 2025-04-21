# invoice_processor/urls.py
from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import (
    InvoiceDocumentUploadView,
    InvoiceTypeViewSet,
    ExtractedDataViewSet,
    InvoiceDocumentStatusView
)

# El router genera automáticamente las URLs para los ViewSets (list, create, retrieve, update, delete)
router = DefaultRouter()
router.register(r'invoice-types', InvoiceTypeViewSet, basename='invoicetype')
router.register(r'extracted-data', ExtractedDataViewSet, basename='extracteddata')
# No registramos InvoiceDocument aquí porque tenemos vistas separadas para subida y estado

urlpatterns = [
    # Ruta para subir nuevos documentos
    path('upload/', InvoiceDocumentUploadView.as_view(), name='invoice-upload'),
    # Ruta para obtener el estado/detalle de un documento específico por su ID
    path('documents/<int:id>/status/', InvoiceDocumentStatusView.as_view(), name='invoice-document-status'),
    # Incluye las URLs generadas por el router para InvoiceType y ExtractedData
    path('', include(router.urls)),
]

# El prefijo de la API (ej. /api/v1/invoices/) se definirá en GARCA/urls.py
