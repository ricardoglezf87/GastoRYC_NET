from django.urls import path
from . import views

urlpatterns = [
    path('import_movements/', views.BankImportView.as_view(), name='bank_import'),
    path('bank_import_preview/', views.BankImportPreviewView.as_view(), name='bank_import_preview'),
]