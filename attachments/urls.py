from django.urls import path
from . import views

urlpatterns = [
    path('upload_attachments/<int:account_id>/', views.upload_attachments, name='upload_attachments'),
    path('upload_entry_attachments/<int:entry_id>/', views.upload_entry_attachments, name='upload_entry_attachments'),
    path('update_attachment_description/<int:attachment_id>/', views.update_attachment_description, name='update_attachment_description'),
    path('delete_attachment/<int:attachment_id>/', views.delete_attachment, name='delete_attachment'),
    path('asociar-pdfs/', views.asociar_pdfs_view, name='asociar_pdfs'),
]