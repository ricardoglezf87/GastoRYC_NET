from django.urls import path
from . import views

urlpatterns = [
    path('hello/', views.trigger_hello_world, name='trigger_hello_world'),
]