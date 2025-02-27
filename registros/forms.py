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

TransactionFormSet = inlineformset_factory(Entry, Transaction, form=TransactionForm, extra=1)