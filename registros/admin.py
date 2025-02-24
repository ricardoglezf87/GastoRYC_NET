from django.contrib import admin
from .models import Account, Entry, Transaction
from mptt.admin import MPTTModelAdmin

class AccountAdmin(MPTTModelAdmin):
    list_display = ('name', 'parent')
    mptt_level_indent = 20

class EntryAdmin(admin.ModelAdmin):
    list_display = ('date', 'description')
    search_fields = ('description',)

class TransactionAdmin(admin.ModelAdmin):
    list_display = ('entry', 'account', 'debit', 'credit')
    search_fields = ('account__name',)

admin.site.register(Account, AccountAdmin)
admin.site.register(Entry, EntryAdmin)
admin.site.register(Transaction, TransactionAdmin)