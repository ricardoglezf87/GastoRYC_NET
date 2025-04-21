from django.shortcuts import render

from GARCA.utils import clear_breadcrumbs


def index(request):
    clear_breadcrumbs(request)
    return render(request, 'admin/index.html')




