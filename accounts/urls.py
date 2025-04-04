from django.urls import path
from . import views

urlpatterns = [
    path('account_tree/', views.account_tree_view, name='account_tree'),
    path('edit_account/<int:account_id>/', views.edit_account, name='edit_account'),
    path('add_account/', views.add_account, name='add_account'),
    path('add_keyword/<int:account_id>/', views.add_keyword, name='add_keyword'),
    path('update_keyword/<int:keyword_id>/', views.update_keyword, name='update_keyword'),
    path('delete_keyword/<int:keyword_id>/', views.delete_keyword, name='delete_keyword'),
    path('get_account_transactions/<int:account_id>/', views.get_account_transactions, name='get_account_transactions'),
]