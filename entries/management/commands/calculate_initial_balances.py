from django.core.management.base import BaseCommand
from accounts.models import Account
from entries.utils import recalculate_balances_after_date
from django.utils import timezone

class Command(BaseCommand):
    help = 'Calcula los balances iniciales para todas las cuentas'

    def handle(self, *args, **kwargs):
        accounts = Account.objects.all()
        for account in accounts:
            recalculate_balances_after_date(
                timezone.datetime(1900, 1, 1).date(),
                account.id
            )
            self.stdout.write(f'Balances calculados para cuenta {account.name}')