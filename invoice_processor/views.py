# invoice_processor/views.py
from rest_framework import viewsets, status, generics
from rest_framework.response import Response
from rest_framework.parsers import MultiPartParser, FormParser
from .models import InvoiceDocument, InvoiceType, ExtractedData
from .serializers import InvoiceDocumentSerializer, InvoiceTypeSerializer, ExtractedDataSerializer
# Importa aquí la lógica de procesamiento (OCR, extracción, etc.) que crearás
# from .processing_logic import process_invoice_file, extract_data_from_text

# Vista para subir documentos
class InvoiceDocumentUploadView(generics.CreateAPIView):
    queryset = InvoiceDocument.objects.all()
    serializer_class = InvoiceDocumentSerializer
    parser_classes = (MultiPartParser, FormParser) # Para manejar subida de archivos

    def perform_create(self, serializer):
        # Guarda el documento inicialmente
        document = serializer.save()
        print(f"Documento {document.id} subido. Iniciando procesamiento...")

        # Aquí iniciarías el procesamiento (puede ser síncrono o asíncrono con Celery)
        # --- Ejemplo Síncrono (simple, puede bloquear si tarda mucho) ---
        try:
            # Llama a tu función de procesamiento (que deberás crear)
            # success, result = process_invoice_file(document.file.path)
            # if success:
            #     document.status = 'PROCESSED' # O 'NEEDS_MAPPING' si no se identificó
            #     document.extracted_text = result.get('text', '')
            #     # Si se identificó tipo y se extrajeron datos, créalos aquí
            #     # ...
            # else:
            #     document.status = 'FAILED'
            # document.save()
            # print(f"Procesamiento de {document.id} completado. Estado: {document.status}")
            # --- Fin Ejemplo Síncrono ---

            # --- Ejemplo Asíncrono (recomendado si el OCR es lento) ---
            # from .tasks import process_invoice_task # Importa tu tarea Celery
            # process_invoice_task.delay(document.id)
            # document.status = 'PROCESSING' # Marcar como en proceso
            # document.save()
            # print(f"Tarea de procesamiento para {document.id} enviada a Celery.")
            # --- Fin Ejemplo Asíncrono ---

            # Por ahora, solo lo marcamos como pendiente
            document.status = 'PENDING'
            document.save()


        except Exception as e:
            print(f"Error iniciando procesamiento para {document.id}: {e}")
            document.status = 'FAILED'
            document.save()
            # Considera devolver un error más específico si falla aquí
            # return Response({"error": "Error al iniciar procesamiento"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR) # Opcional

        # La respuesta inmediata solo confirma la subida
        # La app de escritorio necesitará consultar el estado/resultado después


# ViewSet para gestionar los Tipos de Factura (Plantillas)
class InvoiceTypeViewSet(viewsets.ModelViewSet):
    queryset = InvoiceType.objects.all()
    serializer_class = InvoiceTypeSerializer
    # Aquí podrías añadir permisos si es necesario

# ViewSet para consultar los datos extraídos (solo lectura por ahora)
class ExtractedDataViewSet(viewsets.ReadOnlyModelViewSet):
    queryset = ExtractedData.objects.all()
    serializer_class = ExtractedDataSerializer
    # Podrías añadir filtros, por ejemplo, por documento
    # filterset_fields = ['document']

# Vista para obtener el estado y resultado de un documento específico
class InvoiceDocumentStatusView(generics.RetrieveAPIView):
     queryset = InvoiceDocument.objects.all()
     serializer_class = InvoiceDocumentSerializer
     lookup_field = 'id' # O 'pk'