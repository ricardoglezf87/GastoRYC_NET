from django.urls import path
from . import views

urlpatterns = [
    path('edit_entry/<int:entry_id>/', views.edit_entry, name='edit_entry'),
    path('add_entry/', views.add_entry, name='add_entry'),
]