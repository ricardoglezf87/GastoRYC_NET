from django import forms
from accounts.models import Account

class BankImportForm(forms.Form):
    BANK_CHOICES = (
        ('ing', 'ING Direct'),
        # Puedes añadir más bancos en el futuro
    )
    
    bank_provider = forms.ChoiceField(choices=BANK_CHOICES, label="Proveedor bancario")
    account = forms.ModelChoiceField(queryset=Account.objects.all(), label="Cuenta destino")
    file = forms.FileField(label="Archivo CSV")
