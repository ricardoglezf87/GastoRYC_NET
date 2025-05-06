from PyQt5.QtCore import pyqtSignal, QObject
import os
import traceback
import django

# --- Inicializar Django ---
os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'GARCA.settings')
try:
    django.setup()
    # print("DJANGO SETUP: Django inicializado correctamente.") # Mensaje de confirmación
except Exception as e:
    print(f"DJANGO SETUP ERROR: No se pudo inicializar Django: {e}")
# --------------------------

from .processing_logic import perform_ocr

class ReprocessingWorker(QObject):
    # ... (código sin cambios) ...
    progress_updated = pyqtSignal(int, str)
    file_processed = pyqtSignal(dict) # Reutilizamos la señal
    error_occurred = pyqtSignal(int, str) # Enviamos ID y error
    finished = pyqtSignal()

    def __init__(self, doc_ids, api_client):
        super().__init__()
        self.doc_ids = doc_ids
        self.api_client = api_client
        self._is_running = True

    def run(self):
        total_docs = len(self.doc_ids)
        for i, doc_id in enumerate(self.doc_ids):
            if not self._is_running:
                break
            filename = f"ID {doc_id}" # Más simple
            self.progress_updated.emit(int((i / total_docs) * 100), f"Reprocesando: {filename}")

            try:
                response = self.api_client.reprocess_document(doc_id)
                if response and isinstance(response, dict) and "id" in response and "error" not in response:
                    self.file_processed.emit(response) # Enviar datos actualizados
                else:
                    error_msg = response.get("error", "Error desconocido de API al reprocesar") if isinstance(response, dict) else "Respuesta inválida de API"
                    self.error_occurred.emit(doc_id, error_msg)
            except Exception as e:
                print(f"Error reprocesando {doc_id}: {e}")
                traceback.print_exc()
                self.error_occurred.emit(doc_id, f"Error inesperado: {e}")
            self.progress_updated.emit(int(((i + 1) / total_docs) * 100), f"Reprocesado: {filename}")

        self.progress_updated.emit(100, "Reproceso completado.")
        self.finished.emit()

    def stop(self):
        self._is_running = False

class ProcessingWorker(QObject):
    
    # ... (código sin cambios) ...
    progress_updated = pyqtSignal(int, str)
    file_processed = pyqtSignal(dict)
    error_occurred = pyqtSignal(str, str)
    finished = pyqtSignal()

    def __init__(self, file_paths, api_client): # api_client ahora es el real
        super().__init__()
        self.file_paths = file_paths
        self.api_client = api_client # Guardar el cliente real
        self._is_running = True

    def run(self):
        total_files = len(self.file_paths)
        for i, file_path in enumerate(self.file_paths):
            if not self._is_running: break
            filename = os.path.basename(file_path)
            self.progress_updated.emit(int((i / total_files) * 100), f"Procesando: {filename}")

            try:
                ocr_text = perform_ocr(file_path)
                if ocr_text is not None:
                    response = self.api_client.create_document_with_ocr(file_path, ocr_text)
                    if response and isinstance(response, dict) and "id" in response and "error" not in response:
                        # Añadir filename manualmente ya que la API no lo devuelve en create
                        response['filename'] = filename
                        self.file_processed.emit(response)
                    else:
                        error_msg = response.get("error", "Error desconocido de API") if isinstance(response, dict) else "Respuesta inválida de API"
                        self.error_occurred.emit(filename, error_msg)
                else:
                    self.error_occurred.emit(filename, "Fallo durante el OCR")
            except Exception as e:
                print(f"Error procesando {filename}: {e}")
                traceback.print_exc()
                self.error_occurred.emit(filename, f"Error inesperado: {e}")

            self.progress_updated.emit(int(((i + 1) / total_files) * 100), f"Procesado: {filename}")

        self.progress_updated.emit(100, "Proceso completado.")
        self.finished.emit()

    def stop(self): self._is_running = False
