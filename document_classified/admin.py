# document_classified/admin.py
from django.contrib import admin
from .models import documentInfo, documentType, ExtractedData

@admin.register(documentInfo)
class documentInfoAdmin(admin.ModelAdmin):
    list_display = ('id', 'file', 'status', 'document_type', 'uploaded_at')
    list_filter = ('status', 'document_type', 'uploaded_at')
    search_fields = ('file', 'extracted_text')
    readonly_fields = ('uploaded_at', 'extracted_text') # Campos que no se editan manualmente aquí

@admin.register(documentType)
class documentTypeAdmin(admin.ModelAdmin):
    list_display = ('name', 'created_at', 'updated_at') # Añade 'account' si lo implementas
    search_fields = ('name',)

@admin.register(ExtractedData)
class ExtractedDataAdmin(admin.ModelAdmin):
    list_display = ('document', 'document_date', 'total_amount', 'extracted_at') # Añade más campos extraídos
    readonly_fields = ('extracted_at',) # Generalmente, estos datos no se editan manualmente

