from django.shortcuts import render, get_object_or_404
from .models import Entry, Account

def entry_detail_view(request, entry_id):
    entry = get_object_or_404(Entry, id=entry_id)
    return render(request, 'admin/entry_detail.html', {'entry': entry})

def account_tree_view(request):
    accounts = Account.objects.filter(parent=None).prefetch_related('children')
    return render(request, 'admin/account_tree.html', {'accounts': accounts})