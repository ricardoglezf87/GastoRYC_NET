# document_classified/urls.py
from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import (    
    CreateDocumentWithOCRView,
    FinalizedocumentAttachmentView,
    documentTypeViewSet,
    ExtractedDataViewSet,
    documentInfoStatusView,
    documentInfoListView,
    ReprocessDocumentView,
    TestExtractionRulesView
)

router = DefaultRouter()
router.register(r'document-types', documentTypeViewSet, basename='documenttype')
router.register(r'extracted-data', ExtractedDataViewSet, basename='extracteddata')

urlpatterns = [
    path('', include(router.urls)),
    path('test-rules/', TestExtractionRulesView.as_view(), name='test-extraction-rules'),
    path('documents/', documentInfoListView.as_view(), name='document-document-list'), 
    path('documents/<int:id>/reprocess/', ReprocessDocumentView.as_view(), name='document-document-reprocess'), 
    path('documents/create_with_ocr/', CreateDocumentWithOCRView.as_view(), name='create-document-with-ocr'),
    path('documents/<int:id>/status/', documentInfoStatusView.as_view(), name='document-document-status'),    
    path('documents/<int:document_id>/finalize_attachment/<int:entry_id>/',FinalizedocumentAttachmentView.as_view(),name='finalize_document_attachment'),
]

