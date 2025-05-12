from django import forms
from accounts.models import Account

class BankImportForm(forms.Form):
    BANK_CHOICES = [
        ('ing', 'ING Direct'),
        ('edenred', 'Edenred'),
        ('paypal', 'PayPal'),
    ]
    
    bank_provider = forms.ChoiceField(choices=BANK_CHOICES, label="Proveedor bancario")
    account = forms.ModelChoiceField(
        queryset=Account.objects.all().order_by('name'),
        label="Cuenta destino"
    )
    file = forms.FileField(label="Archivo (CSV, TXT o XLSX según proveedor)")

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.fields['account'].label_from_instance = lambda obj: obj.get_full_hierarchy()
