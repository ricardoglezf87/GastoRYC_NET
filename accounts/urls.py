from django.urls import path
from . import views

urlpatterns = [
    path('account_tree/', views.account_tree_view, name='account_tree'),
    path('edit_account/<int:account_id>/', views.edit_account, name='edit_account'),
    path('add_account/', views.add_account, name='add_account'),
]