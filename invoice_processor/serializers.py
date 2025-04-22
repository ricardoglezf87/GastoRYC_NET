# invoice_processor/serializers.py
from rest_framework import serializers

from entries.models import Entry
from transactions.models import Transaction
from .models import InvoiceDocument, InvoiceType, ExtractedData

class ExtractedDataSerializer(serializers.ModelSerializer):
    class Meta:
        model = ExtractedData
        fields = '__all__' # O lista explícitamente los campos que quieres exponer

class TransactionSerializer(serializers.ModelSerializer):
    # Opcional: Mostrar nombre de cuenta en lugar de ID
    account_name = serializers.CharField(source='account.get_full_hierarchy', read_only=True)

    class Meta:
        model = Transaction
        # Incluye los campos que quieres de cada transacción
        fields = ['id', 'account', 'account_name', 'debit', 'credit']
        read_only_fields = ['account_name'] 
class EntrySerializer(serializers.ModelSerializer):
    # Puedes añadir campos extra o modificar la representación aquí si es necesario
    
    transactions = TransactionSerializer(many=True, read_only=True)
    
    class Meta:
        model = Entry
        fields = ['id', 'date', 'description', 'transactions']

    

class InvoiceTypeSerializer(serializers.ModelSerializer):
    class Meta:
        model = InvoiceType
        # Asegúrate de incluir 'extraction_rules'
        fields = [
            'id',
            'name',
            'extraction_rules', # <-- Incluir el campo JSON
            'account',
            'created_at',
            'updated_at',
            # Incluye otros campos que necesites exponer
        ]
        read_only_fields = ['id', 'created_at', 'updated_at']

class InvoiceDocumentSerializer(serializers.ModelSerializer):
    # Para mostrar los datos extraídos anidados al consultar un documento
    extracted_data = ExtractedDataSerializer(read_only=True)
    # Para mostrar el nombre del tipo de factura en lugar de solo el ID
    invoice_type_name = serializers.CharField(source='invoice_type.name', read_only=True, allow_null=True)
    # Hacer el campo 'file' de solo escritura en la creación, pero legible después
    invoice_type_account_id = serializers.IntegerField(source='invoice_type.account_id', read_only=True, allow_null=True)
    file = serializers.FileField(write_only=True, required=True)
    file_url = serializers.SerializerMethodField(read_only=True) # Campo para mostrar URL del archivo

    class Meta:
        model = InvoiceDocument
        # Lista los campos que quieres en la API
        fields = [
            'id',
            'uploaded_at',
            'file', # Para la subida
            'file_url', # Para la lectura
            'status',
            'get_status_display', # Muestra el texto legible del estado
            'extracted_text',
            'invoice_type', # ID del tipo asociado (para lectura/asignación)
            'invoice_type_name', # Nombre del tipo asociado (solo lectura)
            'invoice_type_account_id', # <-- AÑADIR ESTE CAMPO A LA LISTA
            'extracted_data', # Datos extraídos anidados (solo lectura)
        ]
        # Mueve invoice_type_account_id a read_only_fields si ya estaba definido como read_only
        read_only_fields = ['id', 'uploaded_at', 'status', 'get_status_display', 'extracted_text', 'extracted_data', 'invoice_type_name', 'file_url', 'invoice_type_account_id']

    def get_file_url(self, obj):
        request = self.context.get('request')
        if obj.file and request:
            return request.build_absolute_uri(obj.file.url)
        return None

    def create(self, validated_data):
        # DRF maneja la subida del archivo automáticamente si usas FileField
        # La lógica de procesamiento se llama desde la vista (perform_create)
        return super().create(validated_data)
