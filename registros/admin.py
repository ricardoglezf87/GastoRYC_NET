from django.contrib import admin
from .models import Account, Entry, Transaction
from mptt.admin import MPTTModelAdmin

class TransactionInline(admin.TabularInline):
    model = Transaction
    extra = 1
    autocomplete_fields = ['account']  # Enable autocomplete for account field

class ReadOnlyTransactionInline(admin.TabularInline):
    model = Transaction
    extra = 0
    readonly_fields = ('entry', 'debit', 'credit', 'get_balance')
    can_delete = False
    autocomplete_fields = ['account']  # Enable autocomplete for account field

    def get_balance(self, obj):
        return obj.account.get_balance()
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