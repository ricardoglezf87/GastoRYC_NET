from django import forms

class BankImportForm(forms.Form):
    BANK_CHOICES = (
        ('ing', 'ING Direct'),
        # Puedes añadir más bancos en el futuro
    )
    
    bank_provider = forms.ChoiceField(choices=BANK_CHOICES, label="Proveedor bancario")
    account = forms.ChoiceField(choices=[], label="Cuenta destino")  # Cambiado a ChoiceField
    file = forms.FileField(label="Archivo CSV")
