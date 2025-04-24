# document_classified/views.py
from decimal import Decimal, InvalidOperation
import json
import os
import traceback

# Imports de Django y DRF
from django.db import transaction
from django.utils.dateparse import parse_date
from django.contrib.contenttypes.models import ContentType
from rest_framework import viewsets, status, generics, mixins # Importar mixins
from rest_framework.response import Response
from rest_framework.parsers import MultiPartParser, FormParser
from rest_framework.views import APIView
from rest_framework.decorators import action # Importar action

# Imports de modelos locales y de otras apps
from attachments.models import Attachment
from entries.models import Entry
from .models import documentInfo, documentType, ExtractedData
from .serializers import documentInfoSerializer, documentTypeSerializer, ExtractedDataSerializer

# Importar lógica de procesamiento
from .processing_logic import (
    extract_document_data,
    identify_document_type,
    perform_ocr,
    process_document_document
)

# --- ViewSet para documentType (Sin cambios significativos, ya era un ViewSet) ---
class documentTypeViewSet(viewsets.ModelViewSet):
    """
    Permite crear, leer, actualizar y eliminar Tipos de Documento (Plantillas).
    """
    queryset = documentType.objects.all().order_by('name')
    serializer_class = documentTypeSerializer
    # Podrías añadir permisos y filtros aquí si es necesario

# --- ViewSet Unificado para documentInfo ---
class documentInfoViewSet(mixins.ListModelMixin,
                          mixins.RetrieveModelMixin,
                          mixins.DestroyModelMixin,
                          # No incluimos CreateModelMixin/UpdateModelMixin directamente
                          # para personalizar 'create' y no permitir 'update' estándar.
                          viewsets.GenericViewSet):
    """
    Gestiona las operaciones principales sobre los documentos:
    - Listar (GET /api/documents/)
    - Ver Detalles (GET /api/documents/{id}/)
    - Crear con OCR (POST /api/documents/)
    - Eliminar (DELETE /api/documents/{id}/)
    - Reprocesar (POST /api/documents/{id}/reprocess/)
    - Finalizar Adjunto (POST /api/documents/{id}/finalize-attachment/{entry_id}/)
    """
    queryset = documentInfo.objects.all().order_by('-uploaded_at')
    serializer_class = documentInfoSerializer
    parser_classes = (MultiPartParser, FormParser) # Necesario para create

    # Reemplaza CreateDocumentWithOCRView (se mapea a POST /api/documents/)
    def create(self, request, *args, **kwargs):
        """
        Crea un nuevo documentInfo a partir de un archivo subido y texto OCR.
        Realiza la identificación y extracción inicial.
        """
        uploaded_file = request.FILES.get('file')
        ocr_text = request.data.get('extracted_text') # Texto OCR viene del cliente

        if not uploaded_file:
            return Response({"error": "Falta el archivo ('file')"}, status=status.HTTP_400_BAD_REQUEST)

        try:
            # Crear instancia inicial del documento
            document = documentInfo(
                file=uploaded_file,
                extracted_text=ocr_text if ocr_text is not None else "",
                # Estado inicial podría ser PENDING o PROCESSING si se procesa aquí
                status='PROCESSING'
            )
            document.save() # Guardar para obtener ID y que el archivo se almacene

            print(f"documentInfoViewSet.create: Documento ID {document.id} creado, iniciando procesamiento...")

            # --- LÓGICA DE IDENTIFICACIÓN Y EXTRACCIÓN ---
            identified_type = None
            extracted_data_dict = {}
            new_status = document.status # Empezar con 'PROCESSING'
            
            if document.extracted_text:
                print(f"Intentando identificar tipo para Doc ID: {document.id}")
                identified_type = identify_document_type(document.extracted_text)

                if identified_type:
                    document.document_type = identified_type
                    print(f"Tipo identificado: {identified_type.name}. Intentando extraer datos...")
                    extracted_data_dict = extract_document_data(document.extracted_text, identified_type)

                    # Comprobar si se extrajo al menos un dato útil
                    if extracted_data_dict.get('document_date') or extracted_data_dict.get('total_amount'):
                        ExtractedData.objects.update_or_create(
                            document=document,
                            defaults={
                                'document_date': extracted_data_dict.get('document_date'),
                                'total_amount': extracted_data_dict.get('total_amount'),
                                # Añadir otros campos extraídos aquí si es necesario
                            }
                        )
                        new_status = 'PROCESSED'
                        print(f"Datos extraídos y guardados para Doc ID: {document.id}")
                    else:
                        new_status = 'NEEDS_MAPPING' # Tipo identificado, pero sin datos clave
                        print(f"Tipo identificado pero sin datos extraídos para Doc ID: {document.id}")
                else:
                    new_status = 'NEEDS_MAPPING' # No se pudo identificar el tipo
                    print(f"No se pudo identificar el tipo para Doc ID: {document.id}")
            else:
                new_status = 'FAILED' # Falló porque no había texto OCR
                document.extracted_text = "Error: No se proporcionó texto OCR o estaba vacío."
                print(f"No hay texto OCR para procesar para Doc ID: {document.id}")

            # Guardar el tipo identificado y el estado final
            document.status = new_status
            document.save(update_fields=['document_type', 'status', 'extracted_text'])
            # -------------------------------------------------

            serializer = self.get_serializer(document)
            # headers = self.get_success_headers(serializer.data) # <-- ASEGÚRATE DE ELIMINAR ESTA LÍNEA
            # Simplemente devuelve la respuesta sin intentar generar headers especiales
            return Response(serializer.data, status=status.HTTP_201_CREATED) # <-- SIN 'headers=headers'

        except Exception as e:
            print(f"Error en documentInfoViewSet.create: {e}")
            traceback.print_exc()
            # Intentar marcar como FAILED si ya se creó el objeto
            if 'document' in locals() and document.pk:
                try:
                    document.status = 'FAILED'
                    if not document.extracted_text: # Guardar error si no hay otro
                         document.extracted_text = f"Error interno: {e}"
                    document.save(update_fields=['status', 'extracted_text'])
                except Exception as save_err:
                     print(f"Error adicional al intentar guardar FAILED status: {save_err}")
            return Response({"error": f"Error interno del servidor al procesar documento: {e}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

    # Reemplaza ReprocessDocumentView (se mapea a POST /api/documents/{id}/reprocess/)
    @action(detail=True, methods=['post'])
    def reprocess(self, request, pk=None):
        """
        Dispara el reprocesamiento de un documento existente, reutilizando el OCR si existe.
        """
        document = self.get_object() # Obtiene el documentInfo por pk (id)
        print(f"Solicitud de reprocesamiento (ViewSet action) para Documento ID: {document.id}")
        try:
            # Llamar a la lógica de procesamiento (que ahora reutiliza OCR)
            success, final_status_or_error = process_document_document(document.id)
            document.refresh_from_db() # Recargar datos actualizados

            if success:
                serializer = self.get_serializer(document)
                return Response(serializer.data, status=status.HTTP_200_OK)
            else:
                # Devolver el mensaje de error específico de process_document_document
                return Response({"error": f"Fallo durante el reprocesamiento: {final_status_or_error}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)
        except Exception as e:
            print(f"Error en documentInfoViewSet.reprocess para ID {pk}: {e}")
            traceback.print_exc()
            # Intentar marcar como FAILED si es un error inesperado
            try:
                document.status = 'FAILED'
                document.extracted_text = f"Error inesperado en reproceso: {e}"
                document.save(update_fields=['status', 'extracted_text'])
            except Exception as save_err:
                 print(f"Error adicional al guardar FAILED status en reprocess: {save_err}")
            return Response({"error": f"Error interno del servidor al reprocesar: {e}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

    # Reemplaza FinalizedocumentAttachmentView
    # Se mapea a POST /api/documents/{id}/finalize-attachment/{entry_id}/
    @action(detail=True, methods=['post'], url_path='finalize-attachment/(?P<entry_id>[0-9]+)')
    @transaction.atomic # Asegura atomicidad
    def finalize_attachment(self, request, pk=None, entry_id=None):
        """
        Crea un adjunto para el asiento 'entry_id' usando el archivo del
        documento 'pk', y luego elimina el documento 'pk'.
        """
        document_document = self.get_object() # Obtiene el documentInfo por pk
        try:
            entry = Entry.objects.get(id=entry_id)
        except Entry.DoesNotExist:
             return Response({"error": f"Asiento {entry_id} no encontrado."}, status=status.HTTP_404_NOT_FOUND)

        if not document_document.file or not document_document.file.name:
            return Response({"error": f"Doc {pk} no tiene archivo asociado."}, status=status.HTTP_400_BAD_REQUEST)

        source_file_field = document_document.file
        original_filename = os.path.basename(source_file_field.name)

        try:
            # Crear instancia de Attachment
            entry_content_type = ContentType.objects.get_for_model(Entry)
            attachment = Attachment(
                content_type=entry_content_type,
                object_id=entry.id,
                description=f"Factura: {original_filename}"
            )
            # Guardar el archivo en el adjunto (Django maneja la copia/ruta)
            attachment.file.save(original_filename, source_file_field, save=True) # save=True para guardar el modelo también
            print(f"Adjunto creado (ID: {attachment.id}) para Asiento {entry_id}. Archivo guardado en: {attachment.file.name}")

            # Eliminar el documentInfo original
            doc_id_deleted = document_document.id
            document_document.delete()
            print(f"documentInfo ID {doc_id_deleted} eliminado.")

            return Response({"status": "ATTACHMENT_CREATED_DOC_DELETED", "attachment_id": attachment.id}, status=status.HTTP_200_OK)

        except Exception as e:
            print(f"Error en finalize_attachment para Asiento {entry_id} desde Documento {pk}: {e}")
            traceback.print_exc()
            # La transacción se revierte automáticamente si hay error
            return Response({"error": f"Error interno durante la finalización: {e}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

# --- Vista para Actualizar Datos Extraídos Manualmente (Se mantiene separada) ---
class UpdateExtractedDataView(APIView):
    """
    Permite actualizar manualmente la fecha o el importe de los datos extraídos
    de un documento mediante una petición PATCH.
    URL: /api/documents/extracted-data/{document_id}/
    Payload: {"document_date": "YYYY-MM-DD"} o {"total_amount": "123.45"}
    """
    def patch(self, request, document_id, *args, **kwargs):
        try:
            doc = documentInfo.objects.get(id=document_id)
        except documentInfo.DoesNotExist:
            return Response({"error": f"Documento con id {document_id} no encontrado"}, status=status.HTTP_404_NOT_FOUND)

        data_to_update = request.data
        if not data_to_update or len(data_to_update) != 1:
            return Response({"error": "Se debe proporcionar exactamente un campo ('document_date' o 'total_amount') para actualizar."}, status=status.HTTP_400_BAD_REQUEST)

        field_name, new_value_str = list(data_to_update.items())[0]

        if field_name not in ['document_date', 'total_amount']:
            return Response({"error": f"Campo '{field_name}' no es editable."}, status=status.HTTP_400_BAD_REQUEST)

        # Obtener o crear ExtractedData asociado al documento
        extracted_data, created = ExtractedData.objects.get_or_create(document=doc)

        validated_value = None
        validation_error = None

        # Validar y convertir el valor recibido
        if field_name == 'document_date':
            validated_value = parse_date(new_value_str)
            if validated_value is None and new_value_str: # Permitir borrar fecha con string vacío
                validation_error = f"Formato de fecha inválido: '{new_value_str}'. Use YYYY-MM-DD o deje vacío."
        elif field_name == 'total_amount':
            if new_value_str: # Si no está vacío, intentar convertir
                try:
                    # Usar Decimal para precisión monetaria
                    validated_value = Decimal(new_value_str.replace(',', '.')) # Flexible con separadores
                except (InvalidOperation, ValueError):
                    validation_error = f"Valor de importe inválido: '{new_value_str}'. Debe ser numérico o vacío."
            else: # Si está vacío, se interpreta como borrar el importe (None)
                 validated_value = None

        if validation_error:
            return Response({"error": validation_error}, status=status.HTTP_400_BAD_REQUEST)

        # Actualizar el campo validado en el objeto ExtractedData
        setattr(extracted_data, field_name, validated_value)

        try:
            extracted_data.save()
            print(f"ExtractedData para Doc ID {document_id} actualizado: {field_name}={validated_value}")

            # Opcional: Actualizar estado del documento si ahora tiene ambos datos
            # Recargar para asegurar que tenemos los últimos datos guardados
            extracted_data.refresh_from_db()
            if extracted_data.document_date and extracted_data.total_amount is not None:
                # Cambiar a PROCESSED si estaba en un estado que indica datos faltantes/error
                if doc.status in ['NEEDS_MAPPING', 'FAILED', 'Faltan Datos Extracción']: # Ajusta según tus estados
                    doc.status = 'PROCESSED'
                    doc.save(update_fields=['status'])
                    print(f"Estado de Doc ID {document_id} actualizado a PROCESSED.")
            # Si se borra un dato, quizás debería volver a NEEDS_MAPPING? (Considerar)
            # elif doc.status == 'PROCESSED' and (not extracted_data.document_date or extracted_data.total_amount is None):
            #      doc.status = 'NEEDS_MAPPING'
            #      doc.save(update_fields=['status'])
            #      print(f"Estado de Doc ID {document_id} actualizado a NEEDS_MAPPING por falta de datos.")

            # Devolver el documentInfo completo y actualizado para refrescar el frontend
            doc.refresh_from_db() # Asegurar que el estado está actualizado
            serializer = documentInfoSerializer(doc, context={'request': request})
            return Response(serializer.data, status=status.HTTP_200_OK)

        except Exception as e:
            print(f"Error guardando ExtractedData para Doc ID {document_id}: {e}")
            traceback.print_exc()
            return Response({"error": f"Error interno al guardar datos: {e}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)


# --- Vista para Probar Reglas (Se mantiene separada) ---
class TestExtractionRulesView(APIView):
    """
    Permite probar reglas de extracción enviando un archivo y las reglas JSON.
    Realiza OCR en el servidor y aplica las reglas al texto resultante.
    URL: /api/documents/test-rules/
    Método: POST
    Payload: multipart/form-data con 'file' (archivo) y 'rules' (string JSON)
    """
    parser_classes = (MultiPartParser, FormParser)

    def post(self, request, *args, **kwargs):
        uploaded_file = request.FILES.get('file')
        rules_json_string = request.data.get('rules')

        if not uploaded_file:
            return Response({"error": "Falta el archivo ('file')"}, status=status.HTTP_400_BAD_REQUEST)
        if not rules_json_string:
            return Response({"error": "Faltan las reglas ('rules') en formato JSON string"}, status=status.HTTP_400_BAD_REQUEST)

        try:
            # Parsear las reglas JSON
            rules = json.loads(rules_json_string)
            if not isinstance(rules, dict):
                 raise ValueError("Las reglas deben ser un objeto JSON.")
        except (json.JSONDecodeError, ValueError) as e:
            return Response({"error": f"Error al parsear las reglas JSON: {e}"}, status=status.HTTP_400_BAD_REQUEST)

        try:
            # Realizar OCR en el archivo subido
            print(f"TestExtractionRulesView: Realizando OCR en archivo '{uploaded_file.name}'...")
            # perform_ocr debe poder manejar el objeto UploadedFile de Django
            extracted_text = perform_ocr(uploaded_file)

            if extracted_text is None:
                 # perform_ocr debería haber logueado el error específico
                 return Response({"error": "Fallo durante el OCR en el servidor."}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

            # Ejecutar la extracción con las reglas y el texto OCR
            print("TestExtractionRulesView: Extrayendo datos con reglas proporcionadas...")
            # Pasamos las reglas directamente, extract_document_data puede manejar un dict
            extracted_data = extract_document_data(extracted_text, rules)

            # Devolver los datos extraídos (o un diccionario vacío si no se extrajo nada)
            print(f"TestExtractionRulesView: Datos extraídos: {extracted_data}")
            # Añadir el texto OCR a la respuesta para que el cliente lo vea también
            response_data = {
                "extracted_data": extracted_data,
                "ocr_text": extracted_text # Devolver también el texto para depuración
            }
            return Response(response_data, status=status.HTTP_200_OK)

        except Exception as e:
            print(f"Error inesperado en test-rules: {type(e).__name__}: {e}")
            traceback.print_exc()
            return Response({"error": f"Error procesando las reglas en el servidor: {e}"}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

