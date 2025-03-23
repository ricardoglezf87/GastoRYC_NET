from django.urls import path
from . import views
from .views import BankImportPreviewView

urlpatterns = [
    path('account_tree/', views.account_tree_view, name='account_tree'),
    path('edit_account/<int:account_id>/', views.edit_account, name='edit_account'),
    path('upload_attachments/<int:account_id>/', views.upload_attachments, name='upload_attachments'),
    path('add_account/', views.add_account, name='add_account'),
    path('edit_entry/<int:entry_id>/', views.edit_entry, name='edit_entry'),
    path('add_entry/', views.add_entry, name='add_entry'),
    path('upload_entry_attachments/<int:entry_id>/', views.upload_entry_attachments, name='upload_entry_attachments'),
    path('update_transaction/<int:transaction_id>/', views.update_transaction, name='update_transaction'),
    path('add_transaction/', views.add_transaction, name='add_transaction'),    
    path('delete_transaction/<int:transaction_id>/', views.delete_transaction, name='delete_transaction'),
    path('update_attachment_description/<int:attachment_id>/', views.update_attachment_description, name='update_attachment_description'),
    path('delete_attachment/<int:attachment_id>/', views.delete_attachment, name='delete_attachment'),
    path('import_movements/', views.BankImportView.as_view(), name='bank_import'),
    path('bank_import_preview/', BankImportPreviewView.as_view(), name='bank_import_preview'),
]