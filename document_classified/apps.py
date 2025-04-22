# document_classified/apps.py
from django.apps import AppConfig

class documentProcessorConfig(AppConfig):
    default_auto_field = 'django.db.models.BigAutoField'
    name = 'document_classified'
    verbose_name = "Procesador de Documentos" # Nombre legible para el admin

