from django.urls import path
from . import views

urlpatterns = [
    path('hello/', views.trigger_hello_world, name='trigger_hello_world'),
    path('calculate_inicial_balances/', views.trigger_calculate_inicial_balances, name='calculate_inicial_balances'),
    path('calculate_balance_after_date/<str:date>/<int:account_id>/', views.trigger_calculate_balance_after_date, name='calculate_balance_after_date'),
    path('create_backup/', views.trigger_create_backup_view, name='create_backup'),
]