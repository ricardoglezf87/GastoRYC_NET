from django.urls import path
from .views import account_tree_view, entry_detail_view, edit_account

urlpatterns = [
    path('account_tree/', account_tree_view, name='account_tree'),
    path('entry/<int:entry_id>/', entry_detail_view, name='entry_detail'),
    path('edit_account/<int:account_id>/', edit_account, name='edit_account'),
]