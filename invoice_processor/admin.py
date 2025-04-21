# invoice_processor/admin.py
from django.contrib import admin
from .models import InvoiceDocument, InvoiceType, ExtractedData

@admin.register(InvoiceDocument)
class InvoiceDocumentAdmin(admin.ModelAdmin):
    list_display = ('id', 'file', 'status', 'invoice_type', 'uploaded_at')
    list_filter = ('status', 'invoice_type', 'uploaded_at')
    search_fields = ('file', 'extracted_text')
    readonly_fields = ('uploaded_at', 'extracted_text') # Campos que no se editan manualmente aquí

@admin.register(InvoiceType)
class InvoiceTypeAdmin(admin.ModelAdmin):
    list_display = ('name', 'created_at', 'updated_at') # Añade 'account' si lo implementas
    search_fields = ('name',)

@admin.register(ExtractedData)
class ExtractedDataAdmin(admin.ModelAdmin):
    list_display = ('document', 'invoice_date', 'total_amount', 'extracted_at') # Añade más campos extraídos
    readonly_fields = ('extracted_at',) # Generalmente, estos datos no se editan manualmente

