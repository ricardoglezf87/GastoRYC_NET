# bulk_matcher_app.py
import sys
import os
import django
from tkinter import messagebox
import traceback
from PyQt5.QtWidgets import (
    QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout,
    QPushButton, QTableWidget, QTableWidgetItem, QHeaderView,
    QProgressBar, QLabel, QFileDialog, QSpinBox, QAbstractItemView
)
from PyQt5.QtCore import Qt, QThread, pyqtSignal, QObject
import requests

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'GARCA.settings')
try:
    django.setup()
    print("DJANGO SETUP: Django inicializado correctamente.") # Mensaje de confirmación
except Exception as e:
    print(f"DJANGO SETUP ERROR: No se pudo inicializar Django: {e}")

from .processing_logic import perform_ocr

# --- Importaciones de lógica y API (Ajusta según tu estructura) ---
# Asume que ApiClient está en un archivo accesible o lo defines aquí
# from .api_client import ApiClient # Si está en el mismo paquete
# Asume que perform_ocr está disponible
# from .processing_logic import perform_ocr # Si está en el mismo paquete

# --- Placeholder para ApiClient (si no lo importas) ---
class ApiClient:
    def __init__(self, base_url="http://127.0.0.1:8000/"):
        self.base_url = base_url.rstrip('/')
        self.api_base = f"{self.base_url}/api"
        self.invoices_url = f"{self.api_base}/invoices"
        self.accounts_url = f"{self.api_base}/accounts/" # Corregido para usar api_base
        self.invoice_types_url = f"{self.invoices_url}/invoice-types/"
        self.list_documents_url = f"{self.invoices_url}/documents/"
        self.reprocess_document_url_template = f"{self.invoices_url}/documents/{{id}}/reprocess/"
        self.create_document_ocr_url = f"{self.invoices_url}/documents/create_with_ocr/"
        # --- URLs para búsqueda y asociación ---
        self.search_entries_url = f"{self.api_base}/accounting/entries/search/" # Asume esta URL, ajústala
        self.associate_entry_url = f"{self.invoices_url}/documents/associate_entry/" # Asume esta URL

    def _make_request(self, method, url, **kwargs):
        """Método auxiliar para manejar errores comunes de requests."""
        print(f"API Request: {method.upper()} {url}")
        files = kwargs.get('files')
        if files:
            print(f"API Request Files: {list(files.keys())}")
            kwargs.pop('json', None)
        else:
            headers = kwargs.get('headers', {})
            if 'Content-Type' not in headers and 'json' in kwargs:
                 headers['Content-Type'] = 'application/json'
            kwargs['headers'] = headers

        try:
            # Aumentar timeout un poco más por si el guardado/procesamiento inicial tarda
            response = requests.request(method, url, timeout=60, **kwargs)
            response.raise_for_status()

            if response.status_code == 204:
                 print("API Response: Success (204 No Content)")
                 return True

            if response.content:
                try:
                    json_response = response.json()
                    print(f"API Response (JSON): {json_response}")
                    return json_response
                except requests.exceptions.JSONDecodeError:
                    print(f"API Response: Success (Status {response.status_code}, No JSON Content)")
                    return True # O response.text si prefieres
            else:
                 print(f"API Response: Success (Status {response.status_code}, Empty Content)")
                 return True

        except requests.exceptions.HTTPError as e:
            error_detail = ""
            try:
                error_data = e.response.json()
                if isinstance(error_data, dict):
                    error_detail = " - " + "; ".join([f"{k}: {v[0] if isinstance(v, list) else v}" for k, v in error_data.items()])
                else: error_detail = f" - {error_data}"
            except: error_detail = f" - {e.response.text}"
            print(f"Error HTTP: {e}{error_detail}")
            return {"error": f"Error del servidor ({e.response.status_code}): {error_detail.strip(' -')}"}
        except requests.exceptions.ConnectionError as e:
            print(f"Error de Conexión: {e}")
            return {"error": f"No se pudo conectar al servidor: {self.base_url}"}
        except requests.exceptions.Timeout as e:
            print(f"Error de Timeout: {e}")
            return {"error": "La petición al servidor tardó demasiado."}
        except requests.exceptions.RequestException as e:
            print(f"Error Inesperado de Requests: {e}")
            return {"error": f"Error de red inesperado: {e}"}

    def get_documents(self):
        """Obtiene la lista de todos los documentos del backend."""
        url = self.list_documents_url
        result = self._make_request('get', url)
        # Devuelve la lista o una lista vacía en caso de error
        if isinstance(result, dict) and "error" in result:
            return []
        return result if isinstance(result, list) else []
    
    def reprocess_document(self, document_id):
        """Solicita al backend reprocesar un documento específico."""
        url = self.reprocess_document_url_template.format(id=document_id)
        # Es una petición POST sin cuerpo (o podrías enviar parámetros si fuera necesario)
        result = self._make_request('post', url)
        # Devuelve la respuesta del backend (el documento actualizado o un error)
        return result

    # --- Métodos existentes (get_accounts, get_invoice_types, etc.) ---
    def get_accounts(self):
        url = self.accounts_url
        result = self._make_request('get', url)
        if isinstance(result, dict) and "error" in result: return []
        return result if isinstance(result, list) else []

    def get_invoice_types(self):
        result = self._make_request('get', self.invoice_types_url)
        if isinstance(result, dict) and "error" in result: return []
        return result if isinstance(result, list) else []

    def get_invoice_type(self, type_id):
        url = f"{self.invoice_types_url}{type_id}/"
        result = self._make_request('get', url)
        return result if isinstance(result, dict) and "error" not in result else None

    def create_invoice_type(self, payload):
        result = self._make_request('post', self.invoice_types_url, json=payload)
        return result if isinstance(result, dict) else None

    def update_invoice_type(self, type_id, payload):
        url = f"{self.invoice_types_url}{type_id}/"
        result = self._make_request('patch', url, json=payload)
        return result if isinstance(result, dict) else None

    # --- NUEVO MÉTODO para guardar documento y OCR ---
    def create_document_with_ocr(self, file_path, ocr_text):
        """
        Envía el archivo PDF y el texto OCR al backend para crear/actualizar
        el InvoiceDocument.
        """
        url = self.create_document_ocr_url
        try:
            with open(file_path, 'rb') as f:
                files_payload = {'file': (os.path.basename(file_path), f)}
                # Enviar el texto OCR como un campo de datos del formulario
                data_payload = {'extracted_text': ocr_text}

                # Realizar la petición POST multipart/form-data
                result = self._make_request('post', url, files=files_payload, data=data_payload)
                return result # Debería devolver el JSON del documento creado/actualizado o un error

        except FileNotFoundError:
             print(f"Error: Archivo no encontrado en create_document_with_ocr: {file_path}")
             return {"error": f"Archivo no encontrado: {file_path}"}
        except Exception as e:
             print(f"Error abriendo o enviando archivo en create_document_with_ocr: {e}")
             traceback.print_exc()
             return {"error": f"Error al procesar archivo local: {e}"}

    # --- NUEVO MÉTODO para buscar asientos ---
    def get_accounting_entries(self, account_id, start_date, end_date, amount=None):
        """Busca asientos contables en el backend."""
        url = self.search_entries_url
        params = {
            'account_id': account_id,
            'start_date': start_date,
            'end_date': end_date,
        }
        if amount is not None:
            params['amount'] = amount # Opcional: filtrar por importe en backend si es posible

        result = self._make_request('get', url, params=params)
        # Asume que devuelve una lista de asientos o un dict con error
        if isinstance(result, dict) and "error" in result:
            return []
        return result if isinstance(result, list) else []

    # --- NUEVO MÉTODO para asociar ---
    def associate_entry(self, document_id, entry_id):
        """Solicita al backend asociar un documento con un asiento."""
        url = self.associate_entry_url
        payload = {
            'document_id': document_id,
            'entry_id': entry_id
        }
        result = self._make_request('post', url, json=payload)
        # Devuelve la respuesta del backend (éxito o error)
        return result
class ReprocessingWorker(QObject):
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

            # Obtener nombre de archivo de los datos guardados (opcional, para status)
            # filename = self.parent().documents_data.get(doc_id, {}).get('filename', f"ID {doc_id}")
            filename = f"ID {doc_id}" # Más simple
            self.progress_updated.emit(int((i / total_docs) * 100), f"Reprocesando: {filename}")

            try:
                # Llamar a la API para reprocesar
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

# --- Worker para procesamiento en segundo plano ---
class ProcessingWorker(QObject):
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
                    # --- LLAMAR AL MÉTODO REAL DE LA API ---
                    response = self.api_client.create_document_with_ocr(file_path, ocr_text)
                    # ---------------------------------------

                    # Procesar respuesta real
                    if response and isinstance(response, dict) and "id" in response and "error" not in response:
                        doc_info = {
                            "id": response["id"],
                            "filename": filename,
                            "status": response.get("status", "OCR_DONE"), # Usar estado devuelto
                            # No necesitamos enviar ocr_text a la tabla principal
                            # "ocr_text": ocr_text,
                            # Otros datos podrían venir de la respuesta si el backend los extrae
                            "invoice_date": response.get("invoice_date"),
                            "total_amount": response.get("total_amount"),
                            "account_id": response.get("account_id"), # Si el backend lo asocia
                            "matched_entry": None,
                        }
                        self.file_processed.emit(doc_info)
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

# --- Ventana Principal ---
class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Conciliador de Facturas Masivo")
        self.setGeometry(100, 100, 1000, 600) # Posición y tamaño inicial

        self.api_client = ApiClient() # Instancia del cliente API
        self.documents_data = {} # Diccionario para guardar datos por ID: {doc_id: doc_info}
        self.processing_thread = None
        self.processing_worker = None

        self.init_ui()
        self.load_initial_documents()

    def init_ui(self):
        # Layout principal
        central_widget = QWidget()
        self.setCentralWidget(central_widget)
        main_layout = QVBoxLayout(central_widget)

        # --- Sección Superior: Controles ---
        controls_layout = QHBoxLayout()
        self.load_button = QPushButton("Cargar Facturas PDF")
        self.load_button.clicked.connect(self.load_files)
        controls_layout.addWidget(self.load_button)

        controls_layout.addWidget(QLabel("Tolerancia Días:"))
        self.days_spinbox = QSpinBox()
        self.days_spinbox.setRange(0, 30) # Rango de días
        self.days_spinbox.setValue(3)     # Valor por defecto
        controls_layout.addWidget(self.days_spinbox)

        self.reprocess_button = QPushButton("Reprocesar Seleccionados")
        self.reprocess_button.clicked.connect(self.reprocess_selected)
        self.reprocess_button.setEnabled(False) # Deshabilitado inicialmente
        controls_layout.addWidget(self.reprocess_button)
        
        self.find_match_button = QPushButton("Buscar Asientos Coincidentes")
        self.find_match_button.clicked.connect(self.find_matching_entries)
        self.find_match_button.setEnabled(False) # Deshabilitado hasta que haya filas seleccionadas
        controls_layout.addWidget(self.find_match_button)

        controls_layout.addStretch(1) # Empuja todo a la izquierda
        main_layout.addLayout(controls_layout)

        # --- Sección Media: Tabla de Documentos ---
        self.table_widget = QTableWidget()
        self.table_widget.setColumnCount(10) # Ajusta número de columnas
        self.table_widget.setHorizontalHeaderLabels([
            "ID", "Archivo", "Estado", "Tipo", "Fecha Factura",
            "Importe Factura", "Cuenta", "Fecha Asiento", "Importe Asiento", "Acción"
        ])
        # Ajustar tamaño de columnas
        self.table_widget.horizontalHeader().setSectionResizeMode(1, QHeaderView.Stretch) # Nombre archivo
        self.table_widget.horizontalHeader().setSectionResizeMode(2, QHeaderView.ResizeToContents) # Estado
        # Ocultar columna ID por defecto (se usa internamente)
        self.table_widget.setColumnHidden(0, True)
        # Permitir selección de filas completas
        self.table_widget.setSelectionBehavior(QAbstractItemView.SelectRows)
        # Conectar selección para habilitar/deshabilitar botón
        self.table_widget.itemSelectionChanged.connect(self.on_selection_changed)
        self.table_widget.cellClicked.connect(self.on_selection_changed)

        main_layout.addWidget(self.table_widget)

        # --- Sección Inferior: Progreso y Estado ---
        status_layout = QHBoxLayout()
        self.progress_bar = QProgressBar()
        status_layout.addWidget(self.progress_bar)
        self.status_label = QLabel("Listo.")
        status_layout.addWidget(self.status_label)
        main_layout.addLayout(status_layout)
    
    def handle_reprocessing_error(self, doc_id, error_msg):
        """Actualiza la fila con estado de error."""
        print(f"Error reprocesando ID {doc_id}: {error_msg}")
        for row in range(self.table_widget.rowCount()):
            id_item = self.table_widget.item(row, 0)
            if id_item and int(id_item.text()) == doc_id:
                status_item = QTableWidgetItem("Error Reproceso")
                status_item.setBackground(Qt.red)
                self.table_widget.setItem(row, 2, status_item) # Columna Estado
                break
    def reprocess_selected(self):
        """Inicia el reprocesamiento para las filas seleccionadas."""
        selected_rows = self.table_widget.selectionModel().selectedRows()
        if not selected_rows:
            return

        doc_ids_to_reprocess = []
        for index in selected_rows:
            id_item = self.table_widget.item(index.row(), 0) # Columna ID
            if id_item:
                try:
                    doc_ids_to_reprocess.append(int(id_item.text()))
                except ValueError:
                    print(f"ID inválido en fila {index.row()}: {id_item.text()}")

        if not doc_ids_to_reprocess:
            return

        print(f"Iniciando reproceso para IDs: {doc_ids_to_reprocess}")
        # Deshabilitar botones
        self.load_button.setEnabled(False)
        self.find_match_button.setEnabled(False)
        self.reprocess_button.setEnabled(False)
        self.status_label.setText("Iniciando reprocesamiento...")
        self.progress_bar.setValue(0)

        # Crear y mover worker a hilo
        self.reprocessing_thread = QThread()
        # Pasar self como parent al worker para acceder a documents_data si es necesario
        self.reprocessing_worker = ReprocessingWorker(doc_ids_to_reprocess, self.api_client)
        self.reprocessing_worker.moveToThread(self.reprocessing_thread)

        # Conectar señales
        self.reprocessing_worker.progress_updated.connect(self.update_progress)
        # Reutilizar update_document_table para actualizar la fila con la nueva info
        self.reprocessing_worker.file_processed.connect(self.update_document_table)
        self.reprocessing_worker.error_occurred.connect(self.handle_reprocessing_error)
        self.reprocessing_worker.finished.connect(self.on_reprocessing_finished)
        self.reprocessing_thread.started.connect(self.reprocessing_worker.run)

        # Iniciar hilo
        self.reprocessing_thread.start()

    def on_reprocessing_finished(self):
        """Se ejecuta cuando el hilo de reprocesamiento termina."""
        self.status_label.setText("Reproceso completado.")
        self.load_button.setEnabled(True)
        self.find_match_button.setEnabled(self.table_widget.selectionModel().hasSelection())
        self.reprocess_button.setEnabled(self.table_widget.selectionModel().hasSelection())

        # Limpiar referencias
        if self.reprocessing_thread:
            self.reprocessing_thread.quit()
            self.reprocessing_thread.wait()
        self.reprocessing_thread = None
        self.reprocessing_worker = None
        print("Hilo de reprocesamiento terminado.")

    def on_selection_changed(self):
        """Habilita/deshabilita botones según la selección."""
        selected_rows = self.table_widget.selectionModel().selectedRows()
        has_selection = bool(selected_rows) # Será True si hay filas seleccionadas, False si no

        # --- AÑADIR ESTE PRINT ---
        print(f"on_selection_changed llamado. Filas seleccionadas: {len(selected_rows)}, Habilitar botones: {has_selection}")
        # --------------------------

        # Habilitar 'Buscar Asientos' si hay selección (podrías añadir lógica de estado)
        self.find_match_button.setEnabled(has_selection)
        # Habilitar 'Reprocesar' si hay selección
        self.reprocess_button.setEnabled(has_selection)

    def load_initial_documents(self):
        """Carga los documentos existentes desde la API al iniciar."""
        self.status_label.setText("Cargando documentos existentes...")
        QApplication.processEvents()
        try:
            documents = self.api_client.get_documents()
            if documents:
                self.table_widget.setRowCount(0) # Limpiar tabla antes de llenar
                self.documents_data = {} # Limpiar datos internos
                print(f"Cargando {len(documents)} documentos existentes...")
                for doc_info in documents:
                    # Asegurarse de que doc_info tenga 'filename' si la API no lo devuelve
                    # (Podría necesitar extraerlo de 'file_url')
                    if 'filename' not in doc_info and 'file_url' in doc_info:
                         try:
                             doc_info['filename'] = os.path.basename(doc_info['file_url'])
                         except:
                             doc_info['filename'] = 'Nombre Desconocido'
                    self.update_document_table(doc_info)
                self.status_label.setText(f"{len(documents)} documentos cargados.")
            else:
                self.status_label.setText("No se encontraron documentos existentes o hubo un error.")
        except Exception as e:
            print(f"Error cargando documentos iniciales: {e}")
            traceback.print_exc()
            self.status_label.setText("Error al cargar documentos.")
            messagebox.critical(self, "Error de Carga", f"No se pudieron cargar los documentos:\n{e}")
    
    def load_files(self):
        """Abre diálogo para seleccionar múltiples PDFs e inicia el procesamiento."""
        options = QFileDialog.Options()
        files, _ = QFileDialog.getOpenFileNames(
            self,
            "Seleccionar Facturas PDF",
            "",
            "Archivos PDF (*.pdf);;Todos los archivos (*)",
            options=options
        )

        if files:
            print(f"Archivos seleccionados: {len(files)}")
            self.load_button.setEnabled(False) # Deshabilitar botón mientras procesa
            self.status_label.setText("Iniciando procesamiento...")
            self.progress_bar.setValue(0)

            # Crear y mover worker a un hilo
            self.processing_thread = QThread()
            self.processing_worker = ProcessingWorker(files, self.api_client)
            self.processing_worker.moveToThread(self.processing_thread)

            # Conectar señales
            self.processing_worker.progress_updated.connect(self.update_progress)
            self.processing_worker.file_processed.connect(self.update_document_table)
            self.processing_worker.error_occurred.connect(self.handle_processing_error)
            self.processing_worker.finished.connect(self.on_processing_finished)
            self.processing_thread.started.connect(self.processing_worker.run)

            # Iniciar el hilo
            self.processing_thread.start()
        else:
            print("Selección de archivo cancelada.")

    def update_progress(self, value, text):
        """Actualiza la barra de progreso y la etiqueta de estado."""
        self.progress_bar.setValue(value)
        self.status_label.setText(text)

    def update_document_table(self, doc_info):
        """Añade o actualiza una fila en la tabla con datos REALES."""
        doc_id = doc_info.get("id")
        if not doc_id:
            print("Error: doc_info recibido sin ID.")
            return

        if 'filename' not in doc_info:
            # Buscar el nombre en los datos que ya teníamos guardados
            existing_data = self.documents_data.get(doc_id, {})
            doc_info['filename'] = existing_data.get('filename', f"ID {doc_id}")

        # Guardar/Actualizar datos internos
        self.documents_data[doc_id] = doc_info
        print(f"Actualizando tabla para Doc ID: {doc_id}, Info: {doc_info}") # DEBUG

        # Buscar si la fila ya existe
        row_index = -1
        for row in range(self.table_widget.rowCount()):
            id_item = self.table_widget.item(row, 0) # Columna ID (oculta)
            if id_item and int(id_item.text()) == doc_id:
                row_index = row
                break

        # Si no existe, añadir nueva fila
        if row_index == -1:
            row_index = self.table_widget.rowCount()
            self.table_widget.insertRow(row_index)
            # Añadir el ID como item (oculto pero accesible)
            id_item = QTableWidgetItem(str(doc_id))
            self.table_widget.setItem(row_index, 0, id_item)

        # --- Llenar celdas con datos de doc_info ---
        # Columna 1: Archivo
        self.table_widget.setItem(row_index, 1, QTableWidgetItem(doc_info.get("filename", "")))
        # Columna 2: Estado (usar el texto legible)
        self.table_widget.setItem(row_index, 2, QTableWidgetItem(doc_info.get("get_status_display", doc_info.get("status", ""))))
        # Columna 3: Tipo (Nombre del tipo)
        self.table_widget.setItem(row_index, 3, QTableWidgetItem(doc_info.get("invoice_type_name", "") or "")) # Usa el nombre si existe
        # Columna 4: Fecha Factura (del extracted_data)
        extracted_data = doc_info.get("extracted_data", {}) # Obtener sub-diccionario
        invoice_date_str = extracted_data.get("invoice_date", "") if extracted_data else ""
        self.table_widget.setItem(row_index, 4, QTableWidgetItem(invoice_date_str or ""))
        # Columna 5: Importe Factura (del extracted_data)
        total_amount = extracted_data.get("total_amount") if extracted_data else None
        total_str = f"{float(total_amount):.2f}" if total_amount is not None else ""
        self.table_widget.setItem(row_index, 5, QTableWidgetItem(total_str))
        # Columna 6: Cuenta (ID de la cuenta asociada al TIPO)
        # Necesitamos obtener el ID de la cuenta del tipo de factura
        # Asumiendo que tu serializer devuelve el ID de la cuenta en 'invoice_type_account_id' o similar
        # O si devuelve el objeto tipo completo, puedes accederlo
        account_id_from_type = doc_info.get("invoice_type_account_id", "") # AJUSTA este nombre de campo
        self.table_widget.setItem(row_index, 6, QTableWidgetItem(str(account_id_from_type) or ""))
        # Columna 7: Fecha Asiento (Vacío inicialmente)
        self.table_widget.setItem(row_index, 7, QTableWidgetItem(""))
        # Columna 8: Importe Asiento (Vacío inicialmente)
        self.table_widget.setItem(row_index, 8, QTableWidgetItem(""))
        # Columna 9: Acción (Vacío inicialmente)
        # El botón se añadirá después si hay coincidencia

        # --- Opcional: Colorear fila según estado ---
        status = doc_info.get("status", "")
        color = None
        if status == 'NEEDS_MATCHING':
            color = Qt.yellow # Amarillo claro
        elif status == 'NEEDS_MAPPING':
            color = Qt.cyan # Cian claro
        elif status == 'ASSOCIATED':
            color = Qt.green # Verde claro
        elif status == 'FAILED' or status == 'Error':
            color = Qt.red # Rojo claro

        if color:
            for col in range(self.table_widget.columnCount()):
                item = self.table_widget.item(row_index, col)
                if not item: # Crear item si no existe para poder colorear
                     item = QTableWidgetItem("")
                     self.table_widget.setItem(row_index, col, item)
                item.setBackground(color)

    def find_matching_entries(self):
        """Busca asientos contables para las filas seleccionadas (usa API real)."""
        selected_rows = self.table_widget.selectionModel().selectedRows()
        if not selected_rows: return
        tolerance_days = self.days_spinbox.value()
        self.status_label.setText(f"Buscando asientos (+/- {tolerance_days} días)...")
        QApplication.processEvents()

        # --- Lógica de búsqueda REAL (aún podría necesitar hilo propio) ---
        for index in selected_rows:
            row = index.row()
            id_item = self.table_widget.item(row, 0)
            if not id_item: continue
            doc_id = int(id_item.text())
            doc_info = self.documents_data.get(doc_id)
            if not doc_info: continue

            # --- Obtener datos REALES de doc_info o tabla ---
            account_id = doc_info.get("account_id") # Asume que el backend lo devuelve
            invoice_date_str = doc_info.get("invoice_date") # Asume formato YYYY-MM-DD
            total_amount = doc_info.get("total_amount")
            # ------------------------------------------------

            if not account_id or not invoice_date_str or total_amount is None:
                 print(f"Faltan datos (cuenta, fecha, importe) para buscar coincidencias para doc {doc_id}")
                 self.table_widget.setItem(row, 2, QTableWidgetItem("Faltan Datos"))
                 continue

            try:
                from datetime import datetime, timedelta
                invoice_date = datetime.strptime(invoice_date_str, "%Y-%m-%d")
                start_date = (invoice_date - timedelta(days=tolerance_days)).strftime("%Y-%m-%d")
                end_date = (invoice_date + timedelta(days=tolerance_days)).strftime("%Y-%m-%d")

                # --- Llamar a la API REAL ---
                entries = self.api_client.get_accounting_entries(account_id, start_date, end_date)
                # ---------------------------

                match_found = None
                if entries: # entries ahora es una lista real
                    for entry in entries:
                        if abs(float(entry.get("amount", 0)) - total_amount) < 0.01:
                            match_found = entry
                            break

                if match_found:
                    # ... (actualizar tabla y añadir botón como antes) ...
                    print(f"Coincidencia encontrada para doc {doc_id}: Asiento ID {match_found['id']}")
                    self.table_widget.setItem(row, 2, QTableWidgetItem("Coincidencia Encontrada"))
                    self.table_widget.setItem(row, 7, QTableWidgetItem(match_found.get("date", "")))
                    self.table_widget.setItem(row, 8, QTableWidgetItem(f"{float(match_found.get('amount', 0)):.2f}"))
                    associate_button = QPushButton("Asociar")
                    associate_button.setProperty("doc_id", doc_id)
                    associate_button.setProperty("entry_id", match_found['id'])
                    associate_button.clicked.connect(self.on_associate_button_clicked)
                    self.table_widget.setCellWidget(row, 9, associate_button)
                else:
                    # ... (limpiar tabla como antes) ...
                    print(f"No se encontraron coincidencias para doc {doc_id}")
                    self.table_widget.setItem(row, 2, QTableWidgetItem("Sin Coincidencia"))
                    self.table_widget.setItem(row, 7, QTableWidgetItem(""))
                    self.table_widget.setItem(row, 8, QTableWidgetItem(""))
                    self.table_widget.removeCellWidget(row, 9)

            except Exception as e:
                 print(f"Error buscando coincidencias para doc {doc_id}: {e}")
                 traceback.print_exc()
                 self.table_widget.setItem(row, 2, QTableWidgetItem("Error Búsqueda"))

        self.status_label.setText("Búsqueda de asientos completada.")

    def on_associate_button_clicked(self):
        """Slot para manejar el clic en 'Asociar' (usa API real)."""
        button = self.sender()
        if not button: return
        doc_id = button.property("doc_id")
        entry_id = button.property("entry_id")
        if doc_id is None or entry_id is None: return

        print(f"Intentando asociar Documento ID: {doc_id} con Asiento ID: {entry_id}")
        self.status_label.setText(f"Asociando doc {doc_id}...")
        QApplication.processEvents()

        # --- Llamar a la API REAL ---
        response = self.api_client.associate_entry(doc_id, entry_id)
        # ---------------------------

        if response and isinstance(response, dict) and response.get("status") == "ASSOCIATED": # Verifica respuesta real
            print("Asociación exitosa.")
            self.status_label.setText(f"Documento {doc_id} asociado.")
            # ... (actualizar tabla como antes) ...
            for row in range(self.table_widget.rowCount()):
                id_item = self.table_widget.item(row, 0)
                if id_item and int(id_item.text()) == doc_id:
                    self.table_widget.setItem(row, 2, QTableWidgetItem("Asociado"))
                    self.table_widget.removeCellWidget(row, 9)
                    if doc_id in self.documents_data:
                        self.documents_data[doc_id]["status"] = "ASSOCIATED"
                    break
        else:
            error_msg = response.get("error", "Error desconocido") if isinstance(response, dict) else "Respuesta inválida"
            print(f"Error en asociación: {error_msg}")
            messagebox.warning(self, "Error Asociación", f"No se pudo asociar el documento {doc_id}:\n{error_msg}")
            self.status_label.setText(f"Error al asociar doc {doc_id}.")

    def handle_processing_error(self, filename, error_msg):
        """Muestra un error y actualiza la tabla."""
        print(f"Error procesando {filename}: {error_msg}")
        # Podrías añadir una fila con estado "Error"
        row_index = self.table_widget.rowCount()
        self.table_widget.insertRow(row_index)
        self.table_widget.setItem(row_index, 0, QTableWidgetItem("-1")) # ID inválido
        self.table_widget.setItem(row_index, 1, QTableWidgetItem(filename))
        self.table_widget.setItem(row_index, 2, QTableWidgetItem("Error"))
        # Colorear fila en rojo (opcional)
        # for col in range(self.table_widget.columnCount()):
        #     item = self.table_widget.item(row_index, col)
        #     if not item: item = QTableWidgetItem("")
        #     item.setBackground(Qt.red)

    def on_processing_finished(self):
        """Se ejecuta cuando el hilo de procesamiento termina."""
        self.status_label.setText("Proceso de carga y OCR completado.")
        self.load_button.setEnabled(True) # Habilitar botón de carga de nuevo

        # Limpiar referencias al hilo y worker
        if self.processing_thread:
            self.processing_thread.quit()
            self.processing_thread.wait()
        self.processing_thread = None
        self.processing_worker = None
        print("Hilo de procesamiento terminado.")

    def find_matching_entries(self):
        """Busca asientos contables para las filas seleccionadas."""
        selected_rows = self.table_widget.selectionModel().selectedRows()
        if not selected_rows:
            return

        tolerance_days = self.days_spinbox.value()
        self.status_label.setText(f"Buscando asientos (+/- {tolerance_days} días)...")
        QApplication.processEvents() # Actualizar UI

        # --- Lógica de búsqueda (simplificada) ---
        # En una app real, esto debería ir en otro hilo si la búsqueda es lenta
        for index in selected_rows:
            row = index.row()
            id_item = self.table_widget.item(row, 0)
            if not id_item: continue
            doc_id = int(id_item.text())
            doc_info = self.documents_data.get(doc_id)

            if not doc_info: continue

            # --- Necesitarías extraer/obtener estos datos ---
            # Esto es un placeholder, necesitarías la lógica real de extracción
            # o recuperarlos del backend si ya se procesaron allí.
            account_id = 1 # Placeholder - Obtener de doc_info o tabla
            invoice_date_str = "2024-11-18" # Placeholder - Obtener de doc_info o tabla
            total_amount = 63.03 # Placeholder - Obtener de doc_info o tabla
            # --- Fin Placeholders ---

            if not account_id or not invoice_date_str or total_amount is None:
                 print(f"Faltan datos (cuenta, fecha, importe) para buscar coincidencias para doc {doc_id}")
                 self.table_widget.setItem(row, 2, QTableWidgetItem("Faltan Datos")) # Actualizar estado
                 continue

            try:
                from datetime import datetime, timedelta
                invoice_date = datetime.strptime(invoice_date_str, "%Y-%m-%d") # Ajusta formato si es necesario
                start_date = (invoice_date - timedelta(days=tolerance_days)).strftime("%Y-%m-%d")
                end_date = (invoice_date + timedelta(days=tolerance_days)).strftime("%Y-%m-%d")

                # Llamar a la API
                entries = self.api_client.get_accounting_entries(account_id, start_date, end_date)

                # Buscar coincidencia de importe (simple, podrías hacerlo más robusto)
                match_found = None
                if entries:
                    for entry in entries:
                        # Compara importes (considera una pequeña tolerancia si es necesario)
                        if abs(entry.get("amount", 0) - total_amount) < 0.01: # Tolerancia de 0.01
                            match_found = entry
                            break # Tomar la primera coincidencia

                if match_found:
                    print(f"Coincidencia encontrada para doc {doc_id}: Asiento ID {match_found['id']}")
                    self.table_widget.setItem(row, 2, QTableWidgetItem("Coincidencia Encontrada"))
                    self.table_widget.setItem(row, 7, QTableWidgetItem(match_found.get("date", "")))
                    self.table_widget.setItem(row, 8, QTableWidgetItem(f"{match_found.get('amount', 0):.2f}"))
                    # Añadir botón "Asociar"
                    associate_button = QPushButton("Asociar")
                    # Guardar IDs en el botón para usarlo en el slot
                    associate_button.setProperty("doc_id", doc_id)
                    associate_button.setProperty("entry_id", match_found['id'])
                    associate_button.clicked.connect(self.on_associate_button_clicked)
                    self.table_widget.setCellWidget(row, 9, associate_button)
                else:
                    print(f"No se encontraron coincidencias para doc {doc_id}")
                    self.table_widget.setItem(row, 2, QTableWidgetItem("Sin Coincidencia"))
                    # Limpiar celdas de asiento y quitar botón si existía
                    self.table_widget.setItem(row, 7, QTableWidgetItem(""))
                    self.table_widget.setItem(row, 8, QTableWidgetItem(""))
                    self.table_widget.removeCellWidget(row, 9)


            except Exception as e:
                 print(f"Error buscando coincidencias para doc {doc_id}: {e}")
                 self.table_widget.setItem(row, 2, QTableWidgetItem("Error Búsqueda"))

        self.status_label.setText("Búsqueda de asientos completada.")

    def on_associate_button_clicked(self):
        """Slot para manejar el clic en el botón 'Asociar'."""
        button = self.sender() # Obtener el botón que fue presionado
        if not button: return

        doc_id = button.property("doc_id")
        entry_id = button.property("entry_id")

        if doc_id is None or entry_id is None:
            print("Error: IDs no encontrados en el botón Asociar.")
            return

        print(f"Intentando asociar Documento ID: {doc_id} con Asiento ID: {entry_id}")
        self.status_label.setText(f"Asociando doc {doc_id}...")
        QApplication.processEvents()

        # Llamar a la API para asociar
        response = self.api_client.associate_entry(doc_id, entry_id)

        if response and response.get("status") == "ASSOCIATED":
            print("Asociación exitosa.")
            self.status_label.setText(f"Documento {doc_id} asociado.")
            # Actualizar tabla: cambiar estado y quitar botón
            for row in range(self.table_widget.rowCount()):
                id_item = self.table_widget.item(row, 0)
                if id_item and int(id_item.text()) == doc_id:
                    self.table_widget.setItem(row, 2, QTableWidgetItem("Asociado"))
                    self.table_widget.removeCellWidget(row, 9) # Quitar botón
                    # Actualizar datos internos
                    if doc_id in self.documents_data:
                        self.documents_data[doc_id]["status"] = "ASSOCIATED"
                        # Podrías guardar el entry_id asociado también
                        # self.documents_data[doc_id]["associated_entry_id"] = entry_id
                    break
        else:
            error_msg = response.get("error", "Error desconocido durante la asociación") if isinstance(response, dict) else "Respuesta inválida de API"
            print(f"Error en asociación: {error_msg}")
            messagebox.warning(self, "Error Asociación", f"No se pudo asociar el documento {doc_id}:\n{error_msg}")
            self.status_label.setText(f"Error al asociar doc {doc_id}.")

    def closeEvent(self, event):
        """Asegurarse de detener los hilos si la ventana se cierra."""
        if self.processing_worker:
            print("Deteniendo hilo de carga...")
            self.processing_worker.stop()
        if self.processing_thread and self.processing_thread.isRunning():
            self.processing_thread.quit(); self.processing_thread.wait()

        if self.reprocessing_worker:
            print("Deteniendo hilo de reproceso...")
            self.reprocessing_worker.stop()
        if self.reprocessing_thread and self.reprocessing_thread.isRunning():
            self.reprocessing_thread.quit(); self.reprocessing_thread.wait()

        event.accept()


# --- Bloque Principal ---
if __name__ == '__main__':
    # Configurar Tesseract (si es necesario y no está en el PATH)
    # import pytesseract
    # pytesseract.pytesseract.tesseract_cmd = r'ruta/a/tesseract.exe'
    # os.environ['TESSDATA_PREFIX'] = r'ruta/a/tessdata'

    app = QApplication(sys.argv)
    main_window = MainWindow()
    main_window.show()
    sys.exit(app.exec_())
