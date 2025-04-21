# invoice_processor/urls.py
from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import (
    AssociateEntryView,
    CreateDocumentWithOCRView,
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
    path('accounting/entries/search/', SearchAccountingEntriesView.as_view(), name='search-accounting-entries'), # Ajusta prefijo si quieres
    path('documents/associate_entry/', AssociateEntryView.as_view(), name='associate-entry'),
]

