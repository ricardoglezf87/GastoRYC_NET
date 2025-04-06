from django.db import models
from django.utils import timezone
from django.contrib.contenttypes.fields import GenericRelation
from django.db.models import Sum, F
from attachments.models import Attachment
from decimal import Decimal

def get_default_balance():
    return {}

class Entry(models.Model):
    date = models.DateField(default=timezone.now)
    description = models.CharField(max_length=255)
    attachments = GenericRelation(Attachment)
    balance = models.JSONField(default=get_default_balance)  # Cambiamos dict por get_default_balance

    def __str__(self):
        return f"{self.date} - {self.description}"

    @property
    def transactions(self):
        return self.transactions.all()

    def update_account_balances(self):
        """Actualiza los balances de todas las cuentas afectadas por esta entrada"""
        affected_accounts = set(t.account_id for t in self.transactions.all())
        for account_id in affected_accounts:
            self.balance[str(account_id)] = float(
                self.transactions.filter(account_id=account_id).aggregate(
                    balance=Sum(F('debit') - F('credit'))
                )['balance'] or 0
            )
        self.save()