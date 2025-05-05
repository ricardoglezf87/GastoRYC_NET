from django.urls import path
from . import views

app_name = 'reports' 

urlpatterns = [
    path('unbalanced_entries/', views.unbalanced_entries_report, name='unbalanced_entries_report'),
    path('recategorized_entries/', views.recategorized_entries, name='recategorized_entries'),
    path('merge_transactions/', views.merge_transactions_view, name='merge_transactions'),
    path('detect_transfers/', views.detect_transfers_view, name='detect_transfers'),
    path('delete_empty_entries/', views.delete_empty_entries_view, name='delete_empty_entries'),
    # URLs para procesar las acciones (POST requests)
    path('process_merge_transactions/', views.process_merge_transactions, name='process_merge_transactions'),
    path('process_simplify_transfers/', views.process_simplify_transfers, name='process_simplify_transfers'),
    path('process_delete_empty_entries/', views.process_delete_empty_entries, name='process_delete_empty_entries'),
]