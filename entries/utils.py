from django.db import transaction
from django.db.models import F, Sum
from .models import Entry

def recalculate_balances_after_date(date, account_id):
    """
    Recalcula los balances de todas las entradas posteriores a una fecha
    para una cuenta específica
    """
    with transaction.atomic():
        entries = Entry.objects.filter(
            date__gte=date,
            transactions__account_id=account_id
        ).order_by('date', 'id')

        # Obtener balance anterior
        previous_balance = Entry.objects.filter(
            date__lt=date,
            transactions__account_id=account_id
        ).aggregate(
            balance=Sum(F('transactions__debit') - F('transactions__credit'))
        )['balance'] or 0

        for entry in entries:
            # Asegurarnos de que balance es un diccionario
            if not isinstance(entry.balance, dict):
                entry.balance = {}
                
            # Actualizar balance para esta cuenta
            entry_balance = entry.transactions.filter(
                account_id=account_id
            ).aggregate(
                balance=Sum(F('debit') - F('credit'))
            )['balance'] or 0
            
            previous_balance += entry_balance
            
            # Actualizar el balance en el campo JSON
            entry.balance[str(account_id)] = float(previous_balance)
            entry.save()