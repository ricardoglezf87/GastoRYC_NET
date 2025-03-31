from django.urls import path
from .views import BankImportView, BankImportPreviewView, BankImportDuplicatesView

urlpatterns = [
    path('import_movements/', BankImportView.as_view(), name='bank_import'),
    path('bank_import_preview/', BankImportPreviewView.as_view(), name='bank_import_preview'),
    path('import/duplicates/', BankImportDuplicatesView.as_view(), name='bank_import_duplicates'),
]