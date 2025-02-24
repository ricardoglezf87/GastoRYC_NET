from django.contrib import admin
from .models import Account, Entry, Transaction
from mptt.admin import MPTTModelAdmin
from django import forms
from django.urls import path
from django.shortcuts import render, get_object_or_404

class AccountAdmin(MPTTModelAdmin):
    list_display = ('name', 'get_balance')
    mptt_level_indent = 20
    search_fields = ('name',)  # Enable search by account name

    def get_urls(self):
        urls = super().get_urls()
        custom_urls = [
            path('<int:account_id>/transactions/', self.admin_site.admin_view(self.transactions_view), name='account-transactions'),
        ]
        return custom_urls + urls

    def transactions_view(self, request, account_id):
        account = get_object_or_404(Account, id=account_id)
        transactions = account.transaction_set.all()
        context = dict(
            self.admin_site.each_context(request),
            account=account,
            transactions=transactions,
        )
        return render(request, 'admin/account_transactions.html', context)

class TransactionInline(admin.TabularInline):
    model = Transaction
    extra = 1
    autocomplete_fields = ['account']  # Enable autocomplete for account field

class EntryAdminForm(forms.ModelForm):
    class Meta:
        model = Entry
        fields = '__all__'
        widgets = {
            'description': forms.TextInput(attrs={'size': 80}),
        }

class EntryAdmin(admin.ModelAdmin):
    form = EntryAdminForm
    list_display = ('date', 'description')
    search_fields = ('description',)
    inlines = [TransactionInline]

admin.site.register(Account, AccountAdmin)
admin.site.register(Entry, EntryAdmin)