from django.shortcuts import render

from GARCA.models import Accounts

# Create your views here.

def index(request):
    #item = Accounts(description="Efectivo2")
    #item.save()
    accounts = Accounts.objects.all()        
    
    return render(
            request,
            "GARCA/index.html", 
            {
                'title' : "Cuentas",
                'accounts': accounts
            }
        )