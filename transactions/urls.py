from django.urls import path
from . import views

urlpatterns = [
    path('update_transaction/<int:transaction_id>/', views.update_transaction, name='update_transaction'),
    path('add_transaction/', views.add_transaction, name='add_transaction'),
    path('delete_transaction/<int:transaction_id>/', views.delete_transaction, name='delete_transaction'),
]