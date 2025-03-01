from django.urls import path
from .views import account_tree_view, edit_account, upload_attachments, add_account, edit_entry, add_entry, upload_entry_attachments, update_transaction, add_transaction
from . import views

urlpatterns = [
    path('account_tree/', account_tree_view, name='account_tree'),
    path('edit_account/<int:account_id>/', edit_account, name='edit_account'),
    path('upload_attachments/<int:account_id>/', upload_attachments, name='upload_attachments'),
    path('add_account/', add_account, name='add_account'),
    path('edit_entry/<int:entry_id>/', edit_entry, name='edit_entry'),
    path('add_entry/', add_entry, name='add_entry'),
    path('upload_entry_attachments/<int:entry_id>/', upload_entry_attachments, name='upload_entry_attachments'),
    path('update_transaction/<int:transaction_id>/', update_transaction, name='update_transaction'),
    path('add_transaction/', add_transaction, name='add_transaction'),    
    path('delete_transaction/<int:transaction_id>/', views.delete_transaction, name='delete_transaction'),
    path('update_attachment_description/<int:attachment_id>/', views.update_attachment_description, name='update_attachment_description'),
    path('delete_attachment/<int:attachment_id>/', views.delete_attachment, name='delete_attachment'),
]