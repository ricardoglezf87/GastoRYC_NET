# invoice_processor/apps.py
from django.apps import AppConfig

class InvoiceProcessorConfig(AppConfig):
    default_auto_field = 'django.db.models.BigAutoField'
    name = 'invoice_processor'
    verbose_name = "Procesador de Facturas" # Nombre legible para el admin

