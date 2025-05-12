from django.urls import path
from . import views

app_name = 'reports' 

urlpatterns = [
    path('unbalanced_entries/', views.unbalanced_entries_view, name='unbalanced_entries'),
    path('recategorized_entries/', views.recategorized_entries, name='recategorized_entries'),
    path('merge_transactions/', views.merge_transactions_view, name='merge_transactions'),
    path('detect_transfers/', views.detect_transfers_view, name='detect_transfers'),
    path('delete_empty_entries/', views.delete_empty_entries_view, name='delete_empty_entries'),
    # URLs para procesar acciones (POST requests)
    path('process_merge_transactions/', views.process_merge_transactions, name='process_merge_transactions'),
    path('process_simplify_transfers/', views.process_simplify_transfers, name='process_simplify_transfers'),
    path('process_delete_empty_entries/', views.process_delete_empty_entries, name='process_delete_empty_entries'),
    # URLs para el visor de datos de Looker Studio y actualización de Google Sheet
    path('looker_data_viewer/', views.looker_data_viewer, name='looker_data_viewer'),
    path('trigger_google_sheet_update/', views.trigger_google_sheet_update, name='trigger_google_sheet_update'),
    path('get_google_sheet_update_progress/<str:task_id>/', views.get_google_sheet_update_progress, name='get_google_sheet_update_progress'),

]