from django.db import models
from django.contrib.contenttypes.models import ContentType
from django.contrib.contenttypes.fields import GenericForeignKey

class Attachment(models.Model):
    file = models.FileField(upload_to='adjuntos/')
    content_type = models.ForeignKey(ContentType, on_delete=models.CASCADE, related_name='attachments')
    object_id = models.PositiveIntegerField()
    content_object = GenericForeignKey('content_type', 'object_id')
    description = models.CharField(max_length=255, blank=True, null=True)

    def __str__(self):
        return f"Attachment {self.id}"