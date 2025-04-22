# invoice_processor/views.py
import json
import os
import traceback
from attachments.models import Attachment
from django.contrib.contenttypes.models import ContentType
from entries.models import Entry 
from rest_framework import viewsets, status, generics
from rest_framework.response import Response
from rest_framework.parsers import MultiPartParser, FormParser
from rest_framework.views import APIView
from datetime import datetime, timedelta
from django.db import transaction
from async_tasks.tasks import process_invoice_task
from invoice_processor.processing_logic import extract_invoice_data, identify_invoice_type, perform_ocr, process_invoice_document
from .models import InvoiceDocument, InvoiceType, ExtractedData
from .serializers import EntrySerializer, InvoiceDocumentSerializer, InvoiceTypeSerializer, ExtractedDataSerializer
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
class ReprocessDocumentView(APIView):
    """
    Dispara el reprocesamiento de un documento específico.
    """
    def post(self, request, id, *args, **kwargs): # Recibe ID desde URL
        try:
            document = InvoiceDocument.objects.get(id=id)
            print(f"Solicitud de reprocesamiento para Documento ID: {document.id}")

            # --- Opción 1: Llamar directamente a la lógica síncrona ---
            # Esto bloqueará la respuesta hasta que termine el procesamiento.
            success, final_status = process_invoice_document(document.id)
            # La función process_invoice_document ya guarda el estado final.
            # Recargamos el documento para obtener los datos actualizados por el procesamiento.
            document.refresh_from_db()
            # ---------------------------------------------------------

            # --- Opción 2: Re-encolar en Celery (Comentada) ---
            # from async_tasks.tasks import process_invoice_task # Importa la tarea
            # process_invoice_task.delay(document.id)
            # success = True # Éxito al encolar
            # final_status = 'PROCESSING' # Estado mientras se procesa
            # document.status = final_status
            # document.save(update_fields=['status'])
            # ----------------------------------------------------

            if success:
                # Devolver el documento con su estado y datos finales
                serializer = InvoiceDocumentSerializer(document, context={'request': request})
                return Response(serializer.data, status=status.HTTP_200_OK)
            else:
                # Si process_invoice_document devolvió False
                return Response({"error": f"Fallo durante el reprocesamiento: {final_status}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

        except InvoiceDocument.DoesNotExist:
            return Response({"error": f"Documento con id {id} no encontrado"}, status=status.HTTP_404_NOT_FOUND)
        except Exception as e:
            print(f"Error en ReprocessDocumentView para ID {id}: {e}")
            traceback.print_exc()
            return Response({"error": f"Error interno del servidor al reprocesar: {e}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

class InvoiceDocumentListView(generics.ListAPIView):
    """
    Devuelve una lista de todos los documentos de factura.
    Podrías añadir filtros por estado, fecha, etc. si es necesario.
    """
    queryset = InvoiceDocument.objects.all().order_by('-uploaded_at') # Ordenar por más reciente
    serializer_class = InvoiceDocumentSerializer

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
            extracted_data = extract_invoice_data(extracted_text, temp_type_info)

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

class CreateDocumentWithOCRView(APIView):
    parser_classes = (MultiPartParser, FormParser)
    serializer_class = InvoiceDocumentSerializer # Para la respuesta

    def post(self, request, *args, **kwargs):
        uploaded_file = request.FILES.get('file')
        ocr_text = request.data.get('extracted_text')

        if not uploaded_file:
            return Response({"error": "Falta el archivo ('file')"}, status=status.HTTP_400_BAD_REQUEST)

        try:
            # Crear instancia inicial del documento
            document = InvoiceDocument(
                file=uploaded_file,
                extracted_text=ocr_text if ocr_text is not None else "",
                status='OCR_DONE' # Estado después de OCR externo
            )
            document.save() # Guardar archivo y texto OCR

            # --- LÓGICA DE IDENTIFICACIÓN Y EXTRACCIÓN ---
            identified_type = None
            extracted_data_dict = {}
            new_status = document.status # Empezar con el estado actual

            if document.extracted_text: # Solo intentar si hay texto OCR
                print(f"Intentando identificar tipo para Doc ID: {document.id}")
                identified_type = identify_invoice_type(document.extracted_text)

                if identified_type:
                    document.invoice_type = identified_type
                    print(f"Tipo identificado: {identified_type.name}. Intentando extraer datos...")
                    extracted_data_dict = extract_invoice_data(document.extracted_text, identified_type)

                    if extracted_data_dict:
                        # Guardar datos extraídos en el modelo ExtractedData
                        # Asegúrate que extract_data_based_on_type devuelve los datos parseados correctamente
                        ExtractedData.objects.update_or_create(
                            document=document,
                            defaults={
                                'invoice_date': extracted_data_dict.get('invoice_date'), # Debe ser objeto Date
                                'total_amount': extracted_data_dict.get('total_amount'), # Debe ser Decimal/Float      
                            }
                        )
                        # Actualizar estado: Listo para buscar coincidencias
                        new_status = 'PROCESSED'
                        print(f"Datos extraídos y guardados para Doc ID: {document.id}")
                    else:
                        # Tipo identificado pero no se extrajeron datos
                        new_status = 'NEEDS_MAPPING' # Requiere revisión manual de reglas/datos
                        print(f"Tipo identificado pero sin datos extraídos para Doc ID: {document.id}")
                else:
                    # No se pudo identificar el tipo automáticamente
                    new_status = 'NEEDS_MAPPING'
                    print(f"No se pudo identificar el tipo para Doc ID: {document.id}")
            else:
                # No hay texto OCR para procesar
                new_status = 'FAILED' # O mantener 'OCR_DONE' y marcar error de otra forma
                print(f"No hay texto OCR para procesar para Doc ID: {document.id}")

            # Guardar el tipo identificado y el estado final
            document.status = new_status
            document.save(update_fields=['invoice_type', 'status'])
            # -------------------------------------------------

            # Serializar y devolver el documento actualizado
            # El serializer ahora incluirá el invoice_type_name y extracted_data si existen
            serializer = self.serializer_class(document, context={'request': request})
            return Response(serializer.data, status=status.HTTP_201_CREATED)

        except Exception as e:
            print(f"Error en CreateDocumentWithOCRView: {e}")
            traceback.print_exc()
            # Intentar marcar el documento como fallido si ya se creó
            if 'document' in locals() and document.pk:
                try:
                    document.status = 'FAILED'
                    document.save(update_fields=['status'])
                except: pass # Ignorar errores al intentar guardar el fallo
            return Response({"error": f"Error interno del servidor al procesar documento: {e}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

# --- NUEVA VISTA para buscar asientos (Ejemplo básico) ---
# Necesitarás adaptar esto a tu modelo real de Asientos Contables ('Entry')
class SearchAccountingEntriesView(generics.ListAPIView):
    serializer_class = EntrySerializer # Reemplaza con tu serializer de asientos
    queryset = None # Se filtrará dinámicamente

    def get_serializer_context(self):
        """
        Añade el account_id del filtro al contexto del serializer.
        """
        context = super().get_serializer_context()
        try:
            # Obtener account_id de los parámetros de la query
            context['account_id'] = int(self.request.query_params.get('account_id'))
        except (TypeError, ValueError):
            context['account_id'] = None
        return context

    def get_queryset(self):
        # ... (tu lógica de filtrado existente para obtener el queryset de Entry) ...
        # Asegúrate que esta lógica filtre por account_id usando la relación
        # a través de Transaction, por ejemplo:
        # queryset = Entry.objects.filter(transactions__account_id=account_id, ...)
        # --------------------------------------------------------------------
        account_id = self.request.query_params.get('account_id')
        start_date_str = self.request.query_params.get('start_date')
        end_date_str = self.request.query_params.get('end_date')

        if not account_id or not start_date_str or not end_date_str:
            return Entry.objects.none()

        try:
            account_id = int(account_id)
            start_date = datetime.strptime(start_date_str, "%Y-%m-%d").date()
            end_date = datetime.strptime(end_date_str, "%Y-%m-%d").date()

            # Filtrar Entry basado en la transacción y la fecha
            queryset = Entry.objects.filter(
                transactions__account_id=account_id,
                date__gte=start_date,
                date__lte=end_date
            ).distinct() # distinct() es importante si un Entry pudiera coincidir múltiples veces

            print(f"Buscando asientos para cuenta {account_id} entre {start_date} y {end_date}")
            return queryset

        except (ValueError, TypeError) as e:
            print(f"Error en parámetros de búsqueda de asientos: {e}")
            return Entry.objects.none()
        except Exception as e:
             print(f"Error inesperado buscando asientos: {e}")
             traceback.print_exc()
             return Entry.objects.none()

class FinalizeInvoiceAttachmentView(APIView):
    """
    Crea un Attachment para un Entry usando el archivo de un InvoiceDocument,
    permitiendo que Django determine la ruta final usando upload_to,
    y luego elimina el InvoiceDocument.
    """
    @transaction.atomic # Asegura que toda la operación sea atómica
    def post(self, request, document_id, entry_id, *args, **kwargs):
        try:
            invoice_document = InvoiceDocument.objects.get(id=document_id)
        except InvoiceDocument.DoesNotExist:
             return Response({"error": f"Documento {document_id} no encontrado."}, status=status.HTTP_404_NOT_FOUND)

        try:
            entry = Entry.objects.get(id=entry_id)
        except Entry.DoesNotExist:
             return Response({"error": f"Asiento {entry_id} no encontrado."}, status=status.HTTP_404_NOT_FOUND)

        if not invoice_document.file or not invoice_document.file.name:
            return Response({"error": f"Doc {document_id} no tiene archivo."}, status=status.HTTP_400_BAD_REQUEST)

        # --- Obtener información del archivo fuente ---
        source_file_field = invoice_document.file
        original_filename = os.path.basename(source_file_field.name)

        try:
            # 1. Crear la instancia de Attachment (sin guardar aún)
            entry_content_type = ContentType.objects.get_for_model(Entry)
            attachment = Attachment(
                content_type=entry_content_type,
                object_id=entry.id,
                description=f"Factura: {original_filename}"
                # El campo 'file' se asignará antes de guardar
            )

            # 2. Asignar el archivo fuente al campo 'file' del nuevo adjunto.
            #    Al guardar, Django usará get_attachment_upload_path para la ruta final
            #    y copiará/moverá el archivo según el backend de almacenamiento.
            #    Usar file.save() es más explícito para indicar que se procese el archivo.
            attachment.file.save(original_filename, source_file_field, save=False)
            # Alternativa (puede funcionar pero save() es más seguro):
            # attachment.file = source_file_field

            # 3. Guardar el Attachment (esto ejecuta la lógica de upload_to y la copia/movimiento del archivo)
            attachment.save()
            print(f"Adjunto creado (ID: {attachment.id}) para Asiento {entry_id}. Archivo guardado en: {attachment.file.name}")

            # 4. Eliminar el InvoiceDocument original
            #    Django NO debería eliminar el archivo físico porque 'attachment.file'
            #    ahora lo referencia (especialmente si se copió).
            doc_id_deleted = invoice_document.id
            invoice_document.delete()
            print(f"InvoiceDocument ID {doc_id_deleted} eliminado.")

            # 5. Devolver éxito
            #    Usamos un nuevo estado para claridad
            return Response({"status": "ATTACHMENT_CREATED_DOC_DELETED", "attachment_id": attachment.id}, status=status.HTTP_200_OK)

        except Exception as e:
            print(f"Error al crear adjunto o eliminar Doc para Asiento {entry_id} desde Documento {document_id}: {e}")
            traceback.print_exc()
            # La transacción se revierte automáticamente
            return Response({"error": f"Error interno durante la finalización: {e}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)
