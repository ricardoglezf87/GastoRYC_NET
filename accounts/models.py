from django.db import models
from django.contrib.contenttypes.fields import GenericRelation
from attachments.models import Attachment

class Account(models.Model):
    name = models.CharField(max_length=100)
    parent = models.ForeignKey('self', null=True, blank=True, related_name='children', on_delete=models.CASCADE)
    default_path = models.CharField(max_length=255, blank=True, null=True, verbose_name="Ruta por defecto para adjuntos")
    attachments = GenericRelation(Attachment)
    
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

    def get_balance(self):
        debit_sum = self.transaction_set.aggregate(models.Sum('debit'))['debit__sum'] or 0
        credit_sum = self.transaction_set.aggregate(models.Sum('credit'))['credit__sum'] or 0
        balance = debit_sum - credit_sum

        for child in self.children.all():
            balance += child.get_balance()
        
        balance = round(balance, 2)
        return balance

class AccountKeyword(models.Model):
    account = models.ForeignKey('Account', on_delete=models.CASCADE, related_name='keywords')
    keyword = models.CharField(max_length=100)

    def __str__(self):
        return self.keyword
