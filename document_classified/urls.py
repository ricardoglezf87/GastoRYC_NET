# document_classified/urls.py
from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import (
    CreateDocumentWithOCRView,
    DocumentDetailView,
    FinalizedocumentAttachmentView,
    UpdateExtractedDataView, # <--- Tu vista para PATCH
    documentTypeViewSet,
    ExtractedDataViewSet,    # <--- El ViewSet ReadOnly
    documentInfoStatusView,
    documentInfoListView,
    ReprocessDocumentView,
    TestExtractionRulesView
)

router = DefaultRouter()
router.register(r'document-types', documentTypeViewSet, basename='documenttype')

urlpatterns = [
    # --- MUEVE TU RUTA ESPECÍFICA AQUÍ ARRIBA ---
    path('extracted-data/<int:document_id>/', UpdateExtractedDataView.as_view(), name='update-extracted-data'),
    # -------------------------------------------

    # Incluye las URLs del router (ahora solo para document-types)
    path('', include(router.urls)),

    # El resto de tus URLs específicas
    path('test-rules/', TestExtractionRulesView.as_view(), name='test-extraction-rules'),
    path('documents/', documentInfoListView.as_view(), name='document-document-list'),
    path('documents/<int:id>/', DocumentDetailView.as_view(), name='document-document-detail'),
    path('documents/<int:id>/reprocess/', ReprocessDocumentView.as_view(), name='document-document-reprocess'),
    path('documents/create_with_ocr/', CreateDocumentWithOCRView.as_view(), name='create-document-with-ocr'),
    path('documents/<int:id>/status/', documentInfoStatusView.as_view(), name='document-document-status'),
    path('documents/<int:document_id>/finalize_attachment/<int:entry_id>/',FinalizedocumentAttachmentView.as_view(),name='finalize_document_attachment'),
]
