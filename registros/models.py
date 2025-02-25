from django.db import models
from django.contrib.contenttypes.models import ContentType
from django.contrib.contenttypes.fields import GenericForeignKey, GenericRelation
from mptt.models import MPTTModel, TreeForeignKey
from django.utils import timezone

class Attachment(models.Model):
    file = models.FileField(upload_to='attachments/')
    content_type = models.ForeignKey(ContentType, on_delete=models.CASCADE)
    object_id = models.PositiveIntegerField()
    content_object = GenericForeignKey('content_type', 'object_id')

    def __str__(self):
        return f"Attachment {self.id}"

# ACCOUNT
class Account(MPTTModel):
    name = models.CharField(max_length=100)
    parent = TreeForeignKey('self', on_delete=models.CASCADE, null=True, blank=True, related_name='account_parent')
    attachments = GenericRelation(Attachment)  # Relación genérica con Attachment

    class MPTTMeta:
        order_insertion_by = ['name']

    def __str__(self):
        return self.name

    def get_balance(self):
        debit_sum = self.transaction_set.aggregate(models.Sum('debit'))['debit__sum'] or 0
        credit_sum = self.transaction_set.aggregate(models.Sum('credit'))['credit__sum'] or 0
        balance = debit_sum - credit_sum

        # Include balance of child accounts
        for child in self.get_children():
            balance += child.get_balance()

        return balance

# ENTRY
class Entry(models.Model):
    date = models.DateField(default=timezone.now)
    description = models.CharField(max_length=255)
    attachments = GenericRelation(Attachment)  # Relación genérica con Attachment

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

