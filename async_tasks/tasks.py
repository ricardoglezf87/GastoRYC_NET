from celery import shared_task
from django.db import transaction
from django.db.models import F, Sum
from accounts.models import Account
from entries.models import Entry
from decimal import Decimal
from django.utils import timezone

@shared_task(bind=True)
def hello_world(self):
    print("¡Hola Mundo!")
    return "¡Hola Mundo!"

@shared_task(bind=True)
def calculate_inicial_balances(self):
    print ('Calcula los balances iniciales para todas las cuentas')
    accounts = Account.objects.all()
    for account in accounts:
        recalculate_balances_after_date(
            timezone.datetime(1900, 1, 1).date(),
            account.id
        )
        print(f'Balances calculados para cuenta {account.name}')
    return "Balances iniciales calculados"

@shared_task(bind=True)
def recalculate_balances_after_date(self,date, account_id):
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
    return "Balances recalculados"
