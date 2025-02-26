from django.contrib import admin
from django.contrib.contenttypes.admin import GenericTabularInline
from .models import Account, Entry, Transaction, Attachment
from django.urls import reverse
from django.utils.html import format_html

class AttachmentInline(GenericTabularInline):
    model = Attachment
    extra = 1

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
    can_add = False  # Deshabilitar la capacidad de añadir nuevas transacciones
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

class AccountAdmin(admin.ModelAdmin):
    list_display = ('name', 'get_balance', 'view_entries_link', 'edit_link')
    search_fields = ('name',)
    inlines = [ReadOnlyTransactionInline, AttachmentInline]
    fields = ('name', 'parent')

    def view_entries_link(self, obj):
        url = reverse('entry_detail', args=[obj.id])  # Asegúrate de que el nombre de la URL sea correcto
        return format_html('<a href="{}">Ver Entradas</a>', url)
    view_entries_link.short_description = 'Entradas'

    def edit_link(self, obj):
        url = reverse('edit_account', args=[obj.id])
        return format_html('<a href="{}">Editar</a>', url)
    edit_link.short_description = 'Editar'

    class Media:
        css = {
            'all': ('css/custom_admin.css',)  # Include extra css
        }

class EntryAdmin(admin.ModelAdmin):
    list_display = ('date', 'description')  # Mostrar el campo de archivo adjunto
    search_fields = ('description',)
    inlines = [TransactionInline, AttachmentInline]  # Incluir AttachmentInline
    fields = ('date', 'description')  # Eliminar el campo de archivo adjunto del formulario de edición

    class Media:
        css = {
            'all': ('css/custom_admin.css',)  # Include extra css
        }

admin.site.register(Account, AccountAdmin)
admin.site.register(Entry, EntryAdmin)