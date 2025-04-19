from django.db import models
from django.contrib.contenttypes.fields import GenericRelation
from attachments.models import Attachment

class Account(models.Model):
    name = models.CharField(max_length=100)
    parent = models.ForeignKey('self', null=True, blank=True, related_name='children', on_delete=models.CASCADE)
    default_path = models.CharField(max_length=255, blank=True, null=True, verbose_name="Ruta por defecto para adjuntos")
    closed = models.BooleanField(default=False, verbose_name="Cerrada") 
    attachments = GenericRelation(Attachment)
    balance = models.DecimalField(max_digits=15, decimal_places=2, default=0.00, editable=False)
    
    def __str__(self):
        return self.name
    
    def get_root_parent(self):
        account = self
        while account.parent:
            account = account.parent
        return account
    
    def get_full_hierarchy(self):
        if self.parent:
            return f"{self.parent.get_full_hierarchy()}::{self.name}"
        return self.name

class AccountKeyword(models.Model):
    account = models.ForeignKey('Account', on_delete=models.CASCADE, related_name='keywords')
    keyword = models.CharField(max_length=100)

    def __str__(self):
        return self.keyword
