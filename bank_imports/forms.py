from django import forms
from accounts.models import Account

class BankImportForm(forms.Form):
    BANK_CHOICES = (
        ('ing', 'ING Direct'),
    )
    
    bank_provider = forms.ChoiceField(choices=BANK_CHOICES, label="Proveedor bancario")
    account = forms.ModelChoiceField(
        queryset=Account.objects.all().order_by('name'),
        label="Cuenta destino"
    )
    file = forms.FileField(label="Archivo CSV")

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        # Configurar el label_from_instance para mostrar la jerarquía completa
        self.fields['account'].label_from_instance = lambda obj: obj.get_full_hierarchy()
