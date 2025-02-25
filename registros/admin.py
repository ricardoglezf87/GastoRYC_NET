from django.contrib import admin
from .models import Account, Entry, Transaction
from mptt.admin import MPTTModelAdmin

class TransactionInline(admin.TabularInline):
    model = Transaction
    extra = 1
    autocomplete_fields = ['account']  # Enable autocomplete for account field
    ordering = ('-entry__date', '-id')  # Order by entry date and transaction ID in descending order

class ReadOnlyTransactionInline(admin.TabularInline):
    model = Transaction
    extra = 0
    readonly_fields = ('entry', 'debit', 'credit', 'get_balance')
    can_delete = False
    ordering = ('-entry__date', '-id')  # Order by entry date and transaction ID in ascending order

    def get_balance(self, obj):
        transactions = obj.account.transaction_set.order_by('entry__date', 'id')
        balance = 0
        for transaction in transactions:
            balance += transaction.debit - transaction.credit
            if transaction.id == obj.id:
                return balance
        return balance
    get_balance.short_description = 'Balance'

class AccountAdmin(MPTTModelAdmin):
    list_display = ('name', 'get_balance')
    mptt_level_indent = 20
    search_fields = ('name',)  # Enable search by account name
    inlines = [ReadOnlyTransactionInline]

    class Media:
        css = {
            'all': ('css/custom_admin.css',)  # Include extra css
        }

class EntryAdmin(admin.ModelAdmin):
    list_display = ('date', 'description')
    search_fields = ('description',)
    inlines = [TransactionInline]

    class Media:
        css = {
            'all': ('css/custom_admin.css',)  # Include extra css
        }

admin.site.register(Account, AccountAdmin)
admin.site.register(Entry, EntryAdmin)