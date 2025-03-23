from django import forms
from django.forms import inlineformset_factory
from .models import Account, Entry, Transaction

class AccountForm(forms.ModelForm):
    class Meta:
        model = Account
        fields = ['name', 'parent']

class EntryForm(forms.ModelForm):
    class Meta:
        model = Entry
        fields = ['date', 'description']

class TransactionForm(forms.ModelForm):
    class Meta:
        model = Transaction
        fields = ['account', 'debit', 'credit']

class BankImportForm(forms.Form):
    BANK_CHOICES = (
        ('ing', 'ING Direct'),
        # Puedes añadir más bancos en el futuro
    )
    
    bank_provider = forms.ChoiceField(choices=BANK_CHOICES, label="Proveedor bancario")
    account = forms.ModelChoiceField(queryset=Account.objects.all(), label="Cuenta destino")
    file = forms.FileField(label="Archivo CSV")

TransactionFormSet = inlineformset_factory(Entry, Transaction, form=TransactionForm, extra=1)