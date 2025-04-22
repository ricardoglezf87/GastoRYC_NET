from rest_framework import serializers

from entries.models import Entry
from transactions.serializers import TransactionSerializer

class EntrySerializer(serializers.ModelSerializer):

    transactions = TransactionSerializer(many=True, read_only=True)
    
    class Meta:
        model = Entry
        fields = ['id', 'date', 'description', 'transactions']

    