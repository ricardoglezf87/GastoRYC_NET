from rest_framework import serializers
from transactions.models import Transaction



class TransactionSerializer(serializers.ModelSerializer):
    account_name = serializers.CharField(source='account.get_full_hierarchy', read_only=True)

    class Meta:
        model = Transaction
        fields = ['id', 'account', 'account_name', 'debit', 'credit']
        read_only_fields = ['account_name'] 