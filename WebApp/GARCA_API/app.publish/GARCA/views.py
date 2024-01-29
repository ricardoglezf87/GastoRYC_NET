from django.http import HttpResponse
from django.shortcuts import render

from GARCA.models import Accounts

# Create your views here.

def index(request):
    #item = Accounts(description="Efectivo2")
    #item.save()
    accounts = Accounts.objects.all()
    
    content = "<ul>"
    for account in accounts:
        content += "<li>"+str(account)+"</li>"
    
    content += "</ul>"
    
    return HttpResponse(content)