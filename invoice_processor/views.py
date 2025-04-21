# invoice_processor/views.py
import json
import traceback
from rest_framework import viewsets, status, generics
from rest_framework.response import Response
from rest_framework.parsers import MultiPartParser, FormParser
from rest_framework.views import APIView
from async_tasks.tasks import process_invoice_task
from invoice_processor.processing_logic import extract_data_based_on_type, perform_ocr
from .models import InvoiceDocument, InvoiceType, ExtractedData
from .serializers import InvoiceDocumentSerializer, InvoiceTypeSerializer, ExtractedDataSerializer
# --- Importa la tarea Celery ---

# -----------------------------

# Vista para subir documentos
class InvoiceDocumentUploadView(generics.CreateAPIView):
    queryset = InvoiceDocument.objects.all()
    serializer_class = InvoiceDocumentSerializer
    parser_classes = (MultiPartParser, FormParser)

    def perform_create(self, serializer):
        # Guarda el documento inicialmente con estado PENDING
        document = serializer.save(status='PENDING') # Asegura estado inicial
        print(f"Documento {document.id} subido. Enviando a Celery...")

        try:
            # --- Llama a la tarea Celery ---
            process_invoice_task.delay(document.id)
            # Opcionalmente, actualiza el estado a PROCESSING aquí si quieres reflejarlo inmediatamente
            # document.status = 'PROCESSING'
            # document.save()
            print(f"Tarea de procesamiento para {document.id} enviada a Celery.")
            # -----------------------------

        except Exception as e:
            # Error al ENVIAR la tarea (ej. Redis no disponible)
            print(f"Error enviando tarea a Celery para {document.id}: {e}")
            document.status = 'FAILED' # Marcar como fallo si no se pudo encolar
            document.extracted_text = "Error al encolar tarea de procesamiento"
            document.save()
            # Considera devolver un error 500 aquí para que el cliente sepa que falló el inicio
            # raise APIException("No se pudo iniciar el procesamiento de la factura.") # O algo similar

        # La respuesta inmediata solo confirma la subida (HTTP 201 Created)
        # El cliente (app de escritorio) deberá consultar el estado después usando
        # la vista InvoiceDocumentStatusView.


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
    # filterset_fields = ['document'] # Necesitarías instalar django-filter

# Vista para obtener el estado y resultado de un documento específico
class InvoiceDocumentStatusView(generics.RetrieveAPIView):
     queryset = InvoiceDocument.objects.all()
     serializer_class = InvoiceDocumentSerializer
     lookup_field = 'id' # O 'pk'


class TestExtractionRulesView(APIView):
    # Añadir parsers para manejar multipart/form-data
    parser_classes = (MultiPartParser, FormParser)

    def post(self, request, *args, **kwargs):
        # Obtener el archivo subido
        uploaded_file = request.FILES.get('file')
        # Obtener las reglas (enviadas como string JSON en el campo 'rules')
        rules_json_string = request.data.get('rules')

        if not uploaded_file:
            return Response({"error": "Falta el archivo ('file')"}, status=status.HTTP_400_BAD_REQUEST)
        if not rules_json_string:
            return Response({"error": "Faltan las reglas ('rules')"}, status=status.HTTP_400_BAD_REQUEST)

        try:
            # Parsear las reglas desde la cadena JSON
            rules = json.loads(rules_json_string)
            if not isinstance(rules, dict):
                 raise ValueError("Las reglas deben ser un objeto JSON.")
        except (json.JSONDecodeError, ValueError) as e:
            return Response({"error": f"Error al parsear las reglas JSON: {e}"}, status=status.HTTP_400_BAD_REQUEST)

        try:
            # 1. Realizar OCR directamente sobre el archivo subido en memoria
            #    Necesitas asegurarte que perform_ocr pueda manejar esto.
            #    El objeto UploadedFile de Django se puede pasar a PIL/pdf2image.
            print(f"TestExtractionRulesView: Realizando OCR en archivo '{uploaded_file.name}'...")
            extracted_text = perform_ocr(uploaded_file) # Pasar el objeto UploadedFile

            if extracted_text is None:
                 # perform_ocr debería haber logueado el error específico
                 return Response({"error": "Fallo durante el OCR en el servidor."}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

            # 2. Crear un diccionario temporal para pasar a la función de extracción
            #    No necesitamos un objeto InvoiceType completo.
            temp_type_info = {"extraction_rules": rules}

            # 3. Ejecutar la extracción con las reglas proporcionadas y el texto OCR
            print("TestExtractionRulesView: Extrayendo datos con reglas proporcionadas...")
            extracted_data = extract_data_based_on_type(extracted_text, temp_type_info)

            # 4. Devolver los datos extraídos
            print(f"TestExtractionRulesView: Datos extraídos: {extracted_data}")
            return Response(extracted_data, status=status.HTTP_200_OK)

        # No necesitamos InvoiceDocument.DoesNotExist aquí
        # except InvoiceDocument.DoesNotExist:
        #     return Response({"error": f"Documento con id {document_id} no encontrado"}, status=status.HTTP_404_NOT_FOUND)
        except Exception as e:
            # Loguear el error real en el servidor
            print(f"Error inesperado en test-rules: {type(e).__name__}: {e}")
            traceback.print_exc() # Imprimir traceback completo en logs del servidor
            return Response({"error": f"Error procesando las reglas en el servidor: {e}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

