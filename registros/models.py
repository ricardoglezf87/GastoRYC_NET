from django.db import models
from mptt.models import MPTTModel, TreeForeignKey

#CUENTA
class Cuenta(models.Model):
    nombre = models.CharField(max_length=100)
    saldo = models.DecimalField(max_digits=10, decimal_places=2, default=0)

    def __str__(self):
        return self.nombre


#CATEGORIA
class Categoria(MPTTModel):
    nombre = models.CharField(max_length=100)
    parent = TreeForeignKey('self', on_delete=models.CASCADE, null=True, blank=True, related_name='subcategorias')

    class MOPTTMeta:
        order_insertion_by = ['nombre']

    def __str__(self):
        return self.nombre