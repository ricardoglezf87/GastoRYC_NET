from django.db import models
from django.utils import timezone
from django.contrib.contenttypes.fields import GenericRelation
from attachments.models import Attachment

class Entry(models.Model):
    date = models.DateField(default=timezone.now)
    description = models.CharField(max_length=255)
    attachments = GenericRelation(Attachment)

    def __str__(self):
        return f"{self.date} - {self.description}"