from django.urls import path
from . import views

urlpatterns = [
    path('unbalanced_entries/', views.unbalanced_entries_report, name='unbalanced_entries_report'),
    path('recategorized_entries/', views.recategorized_entries, name='recategorized_entries'),
]