from django.db import models
import uuid

# Create your models here.

class Accounts(models.Model):
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, help_text="ID unico para la tabla")
    description = models.CharField(max_length=200, help_text="Introduce la descripcion")


    class Meta:
        ordering = ["description"]


    def __str__(self):
        """
        String para representar el Objeto del Modelo
        """
        return '%s (%s)' % (self.id,self.description)