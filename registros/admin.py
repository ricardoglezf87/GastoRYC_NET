from django.contrib import admin
from .models import Account, Entry, Transaction
from mptt.admin import MPTTModelAdmin
from django import forms

class AccountAdmin(MPTTModelAdmin):
    list_display = ('name', 'parent')
    mptt_level_indent = 20

class TransactionInline(admin.TabularInline):
    model = Transaction
    extra = 1

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

class TransactionAdmin(admin.ModelAdmin):
    list_display = ('entry', 'account', 'debit', 'credit')
    search_fields = ('account__name',)

admin.site.register(Account, AccountAdmin)
admin.site.register(Entry, EntryAdmin)
admin.site.register(Transaction, TransactionAdmin)