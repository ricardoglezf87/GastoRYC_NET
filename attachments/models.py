import os
from django.db import models
from django.contrib.contenttypes.models import ContentType
from django.contrib.contenttypes.fields import GenericForeignKey

def get_attachment_upload_path(instance, filename):
    """
    Determina la ruta de guardado para un adjunto.
    Si el objeto asociado es una Cuenta o un Asiento con una Cuenta asociada
    que tiene un default_path, lo usa. De lo contrario, usa 'adjuntos/'.
    """
    base_path = 'adjuntos' # Carpeta base por defecto
    specific_path = None
    account = None
    # Mueve las importaciones aquí DENTRO de la función
    from accounts.models import Account
    # Asegúrate que la ruta de importación sea correcta para tu proyecto
    from entries.models import Entry

    # 'instance' es la instancia de Attachment que se está guardando
    # 'instance.content_object' será None al principio, pero Django lo
    # reevaluará después de asignar el content_object en la vista.
    # ¡Es crucial asignar content_object ANTES de llamar a save() en la vista!
    if instance.content_object:
        related_object = instance.content_object

        if isinstance(related_object, Account):
            account = related_object
        elif isinstance(related_object, Entry) and hasattr(related_object, 'account'):
            account = related_object.account
        # Puedes añadir más 'elif' si hay otros modelos relacionados con Account

        if account and account.default_path:
            # Elimina slashes al inicio/final para evitar rutas como 'adjuntos//ruta//archivo.txt'
            specific_path = account.default_path.strip('/')

    # Construye la ruta final
    if specific_path:
        # Une la base, la ruta específica y el nombre de archivo
        # Ej: adjuntos/facturas_proveedor_x/factura_123.pdf
        return os.path.join(base_path, specific_path, filename)
    else:
        # Si no hay ruta específica, guarda en la carpeta base por defecto
        # Ej: adjuntos/factura_123.pdf
        return os.path.join(base_path, filename)


class Attachment(models.Model):
    file = models.FileField(upload_to=get_attachment_upload_path)
    content_type = models.ForeignKey(ContentType, on_delete=models.CASCADE, related_name='attachments')
    object_id = models.PositiveIntegerField()
    content_object = GenericForeignKey('content_type', 'object_id')
    description = models.CharField(max_length=255, blank=True, null=True)

    def __str__(self):
        return f"Attachment {self.id}"
    

