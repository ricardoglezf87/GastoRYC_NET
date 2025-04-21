# invoice_processor/models.py
from django.db import models
from django.conf import settings # Para relacionar con User si es necesario

# Modelo para almacenar las "plantillas" o tipos de factura
# Definimos InvoiceType ANTES de InvoiceDocument para ayudar a Django
class InvoiceType(models.Model):
    name = models.CharField(max_length=100, unique=True, verbose_name="Nombre del Tipo")
    
    # Guarda las reglas de extracción en formato JSON.
    # Ejemplo de estructura:
    # {
    #   "identifier": {"type": "keyword", "value": "Proveedor Ejemplo S.L."},
    #   "date": {"type": "regex", "pattern": "\\d{1,2}[/-]\\d{1,2}[/-]\\d{2,4}"},
    #   "total": {"type": "keyword_proximity", "keyword": "TOTAL", "lines_after": 1},
    #   "coordinates": { # Opcional, si usas mapeo visual
    #       "date": [x1, y1, x2, y2],
    #       "total": [x1, y1, x2, y2]
    #    }
    # }
    extraction_rules = models.JSONField(
        default=dict, # Valor por defecto un diccionario vacío
        blank=True,
        null=True, # Permite nulo si aún no se han definido reglas
        verbose_name="Reglas de Extracción (JSON)"
    )
    
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

    def __str__(self):
        return self.name

    class Meta:
        verbose_name = "Tipo de Factura (Plantilla)"
        verbose_name_plural = "Tipos de Factura (Plantillas)"


# Modelo para almacenar la información sobre el archivo de factura subido
class InvoiceDocument(models.Model):
    STATUS_CHOICES = [
        ('PENDING', 'Pendiente de Procesar'),
        ('PROCESSING', 'Procesando'),
        ('PROCESSED', 'Procesado'),
        ('FAILED', 'Error en Procesamiento'),
        ('NEEDS_MAPPING', 'Necesita Mapeo Manual'),
    ]

    uploaded_at = models.DateTimeField(auto_now_add=True, verbose_name="Fecha de Subida")
    # Puedes relacionarlo con el usuario que lo subió si tienes autenticación
    # uploaded_by = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.SET_NULL, null=True, blank=True)
    file = models.FileField(upload_to='invoices/%Y/%m/%d/', verbose_name="Archivo de Factura")
    status = models.CharField(max_length=20, choices=STATUS_CHOICES, default='PENDING', verbose_name="Estado")
    # Guarda el texto extraído por OCR
    extracted_text = models.TextField(blank=True, null=True, verbose_name="Texto Extraído (OCR)")
    # Relación con el tipo de factura identificado (puede ser nulo al principio)
    # Usar la referencia completa 'invoice_processor.InvoiceType' es más seguro
    invoice_type = models.ForeignKey(
        'invoice_processor.InvoiceType', # Especifica la app
        on_delete=models.SET_NULL,
        null=True,
        blank=True,
        related_name='documents',
        verbose_name="Tipo de Factura"
    )

    def __str__(self):
        # Es bueno incluir el ID para diferenciar instancias fácilmente
        return f"Factura {self.id or '(Nuevo)'} ({self.get_status_display()})"

    class Meta:
        verbose_name = "Documento de Factura"
        verbose_name_plural = "Documentos de Factura"
        ordering = ['-uploaded_at']


# Modelo para almacenar los datos extraídos de una factura específica
class ExtractedData(models.Model):
    # Usar la referencia completa 'invoice_processor.InvoiceDocument' es más seguro
    document = models.OneToOneField(
        'invoice_processor.InvoiceDocument', # Especifica la app
        on_delete=models.CASCADE,
        related_name='extracted_data',
        verbose_name="Documento"
    )
    invoice_date = models.DateField(null=True, blank=True, verbose_name="Fecha Factura")
    total_amount = models.DecimalField(max_digits=10, decimal_places=2, null=True, blank=True, verbose_name="Importe Total")
    # Añade aquí los demás campos que extraigas
    # issuer_vat_id = models.CharField(max_length=50, blank=True, null=True, verbose_name="NIF Emisor")
    # invoice_number = models.CharField(max_length=100, blank=True, null=True, verbose_name="Número Factura")
    # ...

    extracted_at = models.DateTimeField(auto_now_add=True)

    def __str__(self):
        # Acceder al ID del documento relacionado de forma segura
        doc_id = self.document.id if self.document else '??'
        return f"Datos extraídos de Documento {doc_id}"

    class Meta:
        verbose_name = "Dato Extraído"
        verbose_name_plural = "Datos Extraídos"

