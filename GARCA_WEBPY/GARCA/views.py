from django.http import HttpResponse
from django.shortcuts import render

from GARCA.models import Accounts

# Create your views here.

def index(request):
    item = Accounts(description="Efectivo")
    item.save()
    return HttpResponse("Hello, Django!")