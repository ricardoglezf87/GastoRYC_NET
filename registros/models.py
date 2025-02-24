from django.db import models
from mptt.models import MPTTModel, TreeForeignKey
from django.utils import timezone

# ACCOUNT
class Account(MPTTModel):
    name = models.CharField(max_length=100)
    parent = TreeForeignKey('self', on_delete=models.CASCADE, null=True, blank=True, related_name='account_parent')

    class MPTTMeta:
        order_insertion_by = ['name']

    def __str__(self):
        return self.name

# ENTRY
class Entry(models.Model):
    date = models.DateField(default=timezone.now)
    description = models.CharField(max_length=255)

    def __str__(self):
        return f"{self.date} - {self.description}"

# TRANSACTION
class Transaction(models.Model):
    entry = models.ForeignKey(Entry, on_delete=models.CASCADE, related_name='transactions')
    account = models.ForeignKey(Account, on_delete=models.CASCADE)
    debit = models.DecimalField(max_digits=10, decimal_places=2, default=0)
    credit = models.DecimalField(max_digits=10, decimal_places=2, default=0)

    def __str__(self):
        return f"{self.account.name} - Debit: {self.debit}, Credit: {self.credit}"

