# invoice_processor/urls.py
from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import (    
    CreateDocumentWithOCRView,
    FinalizeInvoiceAttachmentView,
    InvoiceDocumentUploadView,
    InvoiceTypeViewSet,
    ExtractedDataViewSet,
    InvoiceDocumentStatusView,
    SearchAccountingEntriesView,
    InvoiceDocumentListView,
    ReprocessDocumentView,
    TestExtractionRulesView
)

router = DefaultRouter()
router.register(r'invoice-types', InvoiceTypeViewSet, basename='invoicetype')
router.register(r'extracted-data', ExtractedDataViewSet, basename='extracteddata')

urlpatterns = [
    path('upload/', InvoiceDocumentUploadView.as_view(), name='invoice-upload'),
    path('', include(router.urls)),
    path('test-rules/', TestExtractionRulesView.as_view(), name='test-extraction-rules'),
    path('documents/', InvoiceDocumentListView.as_view(), name='invoice-document-list'), 
    path('documents/<int:id>/reprocess/', ReprocessDocumentView.as_view(), name='invoice-document-reprocess'), 
    path('documents/create_with_ocr/', CreateDocumentWithOCRView.as_view(), name='create-document-with-ocr'),
    path('documents/<int:id>/status/', InvoiceDocumentStatusView.as_view(), name='invoice-document-status'),
    path('entries/search/', SearchAccountingEntriesView.as_view(), name='search-accounting-entries'),     
    path('documents/<int:document_id>/finalize_attachment/<int:entry_id>/',FinalizeInvoiceAttachmentView.as_view(),name='finalize_invoice_attachment'),
]

