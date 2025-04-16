# c:\Users\rgonzafa\source\repos\GastoRYC_NET\attachments\models.py
import os
from django.db import models
from django.contrib.contenttypes.models import ContentType
from django.contrib.contenttypes.fields import GenericForeignKey


def get_attachment_upload_path(instance, filename):
    """
    Determina la ruta de guardado para un adjunto con prioridades.
    - Si está asociado a una Account, usa su default_path.
    - Si está asociado a un Entry, revisa las cuentas de sus transacciones.
      Busca la cuenta cuya raíz coincida con la lista de prioridades.
      Usa el default_path de la PRIMERA cuenta encontrada que coincida
      con la prioridad más alta.
    - Si no se encuentra ninguna ruta prioritaria, usa 'adjuntos/'.
    """
    # Importaciones diferidas
    from accounts.models import Account
    from entries.models import Entry

    base_path = 'adjuntos'
    specific_path = None

    # Lista de prioridades de nombres de cuentas raíz
    # Asegúrate que estos nombres coincidan EXACTAMENTE con los nombres de tus cuentas raíz
    priority_list = ["Ingresos", "Gastos", "Pasivos", "Activos", "Resultados"]

    if instance.content_object:
        related_object = instance.content_object

        if isinstance(related_object, Account):
            # --- Caso 1: Adjunto directamente en una Cuenta ---
            if related_object.default_path:
                specific_path = related_object.default_path.strip('/')

        elif isinstance(related_object, Entry):
            # --- Caso 2: Adjunto en un Asiento (Entry) ---
            try:
                # Obtener todas las transacciones asociadas al asiento
                # Usamos select_related para optimizar la obtención de la cuenta
                transactions = related_object.transactions.select_related('account').all()

                # Iterar sobre la lista de prioridades
                for priority_name in priority_list:
                    # Iterar sobre las transacciones del asiento
                    for tx in transactions:
                        if tx.account:
                            # Obtener la cuenta raíz de la cuenta de la transacción
                            root_account = tx.account.get_root_parent()
                            # Comprobar si la raíz coincide con la prioridad actual
                            if root_account and root_account.name == priority_name:
                                # ¡Coincidencia! Comprobar si ESTA cuenta (tx.account) tiene default_path
                                if tx.account.default_path:
                                    # Usar la ruta de esta cuenta y detener la búsqueda
                                    specific_path = tx.account.default_path.strip('/')
                                    break # Salir del bucle de transacciones
                    # Si encontramos una ruta para esta prioridad, no necesitamos seguir buscando en prioridades menores
                    if specific_path:
                        break # Salir del bucle de prioridades

            except AttributeError as e:
                # Manejar errores si falta 'transactions', 'account' o 'get_root_parent'
                # Puedes añadir logging aquí: print(f"Error buscando ruta prioritaria: {e}")
                pass # Se usará la ruta base

    # --- Construcción final de la ruta ---
    if specific_path:
        return os.path.join(base_path, specific_path, filename)
    else:
        # Si no se determinó una ruta específica, usa la base
        return os.path.join(base_path, filename)


class Attachment(models.Model):
    file = models.FileField(upload_to=get_attachment_upload_path)
    content_type = models.ForeignKey(ContentType, on_delete=models.CASCADE, related_name='attachments')
    object_id = models.PositiveIntegerField()
    content_object = GenericForeignKey('content_type', 'object_id')
    description = models.CharField(max_length=255, blank=True, null=True)

    def __str__(self):
        base_name = os.path.basename(self.file.name) if self.file else f"Attachment {self.id}"
        return self.description or base_name

    @property
    def full_path(self):
        return self.file.name if self.file else ''

