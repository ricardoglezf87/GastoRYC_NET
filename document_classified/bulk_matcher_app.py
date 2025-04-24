# bulk_matcher_app.py

import sys
import os
import traceback
# <<<--- Añadir datetime para parsear la fecha del asiento si viene como string --- >>>
from datetime import datetime
# <<<--------------------------------------------------------------------------->>>
from PyQt5.QtWidgets import (
    QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout,
    QPushButton, QTableWidget, QTableWidgetItem, QHeaderView,
    QProgressBar, QLabel, QFileDialog, QSpinBox, QAbstractItemView,
    QMessageBox
)
# Asegúrate de importar QUrl
from PyQt5.QtCore import Qt, QThread, QUrl, pyqtSignal
from PyQt5.QtGui import QDesktopServices
# Asumiendo que garca_api_client está en el mismo directorio o en el PYTHONPATH
from .garca_api_client import ApiClient
# Asumiendo que bulk_matcher_ProcessWoker está en el mismo directorio o en el PYTHONPATH
from .bulk_matcher_ProcessWoker import ProcessingWorker, ReprocessingWorker

class DropTableWidget(QTableWidget):
    filesDropped = pyqtSignal(list)

    def __init__(self, parent=None):
        super().__init__(parent)
        self.setAcceptDrops(True)
        self.is_saving_edit = False
        print("DropTableWidget: __init__ - Drops habilitados.") # DEBUG

    def dragEnterEvent(self, event):
        print("DropTableWidget: dragEnterEvent triggered") # DEBUG
        mime_data = event.mimeData()
        print(f"  Mime formats: {mime_data.formats()}") # DEBUG
        print(f"  Mime URLs: {mime_data.urls()}") # DEBUG

        if mime_data.hasUrls():
            has_pdf = False
            for url in mime_data.urls():
                if url.isLocalFile() and url.toLocalFile().lower().endswith('.pdf'):
                    has_pdf = True
                    break
            if has_pdf:
                print("  Tiene PDF. Aceptando acción propuesta.") # DEBUG
                event.acceptProposedAction()
                self.setStyleSheet("background-color: #e0f0ff;") # Azul claro
            else:
                print("  No tiene PDF. Ignorando.") # DEBUG
                event.ignore()
        else:
            print("  No tiene URLs. Ignorando.") # DEBUG
            event.ignore()

    def dragMoveEvent(self, event):
        # Este evento es crucial. Si no se acepta aquí, el cursor puede seguir prohibido.
        if event.mimeData().hasUrls():
            # Comprobar si tiene PDF para mantener consistencia con dragEnter
            has_pdf = False
            for url in event.mimeData().urls():
                if url.isLocalFile() and url.toLocalFile().lower().endswith('.pdf'):
                    has_pdf = True
                    break
            if has_pdf:
                event.acceptProposedAction()
            else:
                event.ignore()
        else:
            event.ignore()

    def dragLeaveEvent(self, event):
        print("DropTableWidget: dragLeaveEvent triggered") # DEBUG
        self.setStyleSheet("")
        event.accept()

    def dropEvent(self, event):
        print("DropTableWidget: dropEvent triggered") # DEBUG
        self.setStyleSheet("")
        mime_data = event.mimeData()
        print(f"  Drop Mime URLs: {mime_data.urls()}") # DEBUG

        if mime_data.hasUrls():
            event.acceptProposedAction()
            file_paths = []
            for url in mime_data.urls():
                if url.isLocalFile():
                    file_path = url.toLocalFile()
                    print(f"  Archivo local detectado: {file_path}") # DEBUG
                    if os.path.isfile(file_path) and file_path.lower().endswith('.pdf'):
                        print(f"    -> Es PDF válido. Añadiendo.") # DEBUG
                        file_paths.append(file_path)
                    else:
                        print(f"    -> Ignorado (no es PDF válido).") # DEBUG
                else:
                    print(f"  URL ignorada (no local): {url.toString()}") # DEBUG

            if file_paths:
                print(f"  Emitiendo señal filesDropped con: {file_paths}") # DEBUG
                self.filesDropped.emit(file_paths)
            else:
                print("  No se encontraron archivos PDF válidos para emitir.") # DEBUG
        else:
            print("  Drop ignorado (no tiene URLs).") # DEBUG
            event.ignore()

# --- Ventana Principal ---
class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Conciliador de Documentos Masivo")
        self.setGeometry(100, 100, 1500, 700) # Ajustar tamaño si es necesario

        self.api_client = ApiClient()
        self.documents_data = {}
        self.processing_thread = None
        self.processing_worker = None
        self.reprocessing_thread = None
        self.reprocessing_worker = None
        self.is_saving_edit = False

        self.init_ui()
        self.load_initial_documents()

    def init_ui(self):
        central_widget = QWidget()
        self.setCentralWidget(central_widget)
        main_layout = QVBoxLayout(central_widget)

        # --- Sección Superior: Controles ---
        controls_layout = QHBoxLayout()
        self.load_button = QPushButton("Cargar documentos PDF")
        self.load_button.clicked.connect(self.load_files_dialog)
        controls_layout.addWidget(self.load_button)

        controls_layout.addWidget(QLabel("Tolerancia Días:"))
        self.days_spinbox = QSpinBox()
        self.days_spinbox.setRange(0, 30)
        self.days_spinbox.setValue(3)
        controls_layout.addWidget(self.days_spinbox)

        self.reprocess_button = QPushButton("Reprocesar Seleccionados")
        self.reprocess_button.clicked.connect(self.reprocess_selected)
        self.reprocess_button.setEnabled(False)
        controls_layout.addWidget(self.reprocess_button)

        self.find_match_button = QPushButton("Buscar Asientos Coincidentes")
        self.find_match_button.clicked.connect(self.find_matching_entries)
        self.find_match_button.setEnabled(False)
        controls_layout.addWidget(self.find_match_button)

        controls_layout.addStretch(1)
        main_layout.addLayout(controls_layout)
        # --- FIN Sección Superior ---

        # --- Sección Media: Tabla de Documentos ---
        self.table_widget = DropTableWidget()
        self.table_widget.filesDropped.connect(self.start_processing_files)
        # <<<--- CAMBIO: Reducir número de columnas a 12 --->>>
        self.table_widget.setColumnCount(12)
        # <<<--- CAMBIO: Quitar cabecera "Importe Asiento" --->>>
        self.table_widget.setHorizontalHeaderLabels([
            "ID", "Archivo", "Estado", "Tipo", "Fecha Factura",
            "Importe Factura", "Cuenta Doc.", "ID Asiento", "Fecha Asiento", # Col 8
            # "Importe Asiento", # Col 9 eliminada
            "Desc. Asiento", # Ahora Col 9
            "Cuentas Asiento", # Ahora Col 10
            "Acción" # Ahora Col 11
        ])
        # <<<-------------------------------------------------------->>>
        # Ajustar resize modes (los índices cambian)
        self.table_widget.horizontalHeader().setSectionResizeMode(1, QHeaderView.Stretch) # Archivo
        self.table_widget.horizontalHeader().setSectionResizeMode(2, QHeaderView.ResizeToContents) # Estado
        self.table_widget.horizontalHeader().setSectionResizeMode(8, QHeaderView.ResizeToContents) # Fecha Asiento
        # self.table_widget.horizontalHeader().setSectionResizeMode(9, QHeaderView.ResizeToContents) # Importe Asiento (Eliminado)
        self.table_widget.horizontalHeader().setSectionResizeMode(9, QHeaderView.Stretch) # Desc. Asiento (Ahora 9)
        self.table_widget.horizontalHeader().setSectionResizeMode(10, QHeaderView.Stretch) # Cuentas Asiento (Ahora 10)
        self.table_widget.horizontalHeader().setSectionResizeMode(11, QHeaderView.ResizeToContents) # Acción (Ahora 11)

        self.table_widget.setColumnHidden(0, True) # Ocultar ID interno
        self.table_widget.setSelectionBehavior(QAbstractItemView.SelectRows)
        self.table_widget.itemSelectionChanged.connect(self.on_selection_changed)
        self.table_widget.cellClicked.connect(self.on_selection_changed)

        self.table_widget.setAcceptDrops(True)
        self.table_widget.itemChanged.connect(self.handle_item_changed)

        main_layout.addWidget(self.table_widget)
        # --- FIN Sección Media ---

        # --- Sección Inferior: Progreso y Estado ---
        status_layout = QHBoxLayout()
        self.progress_bar = QProgressBar()
        status_layout.addWidget(self.progress_bar)
        self.status_label = QLabel("Listo. Puedes arrastrar archivos PDF a la tabla.")
        status_layout.addWidget(self.status_label)
        main_layout.addLayout(status_layout)
        # --- FIN Sección Inferior ---

    # --- Slots y Métodos ---

    def load_files_dialog(self):
        """Abre el diálogo para seleccionar archivos."""
        options = QFileDialog.Options()
        files, _ = QFileDialog.getOpenFileNames(self,"Seleccionar documentos PDF","", "Archivos PDF (*.pdf);;Todos los archivos (*)", options=options)
        if files:
            self.start_processing_files(files)
        else:
            print("Selección de archivo cancelada.")

    def start_processing_files(self, file_paths):
        """Inicia el hilo de procesamiento para una lista de rutas de archivo."""
        if not file_paths:
            return

        if self.processing_thread and self.processing_thread.isRunning():
            QMessageBox.warning(self, "Proceso en Curso", "Ya hay un proceso de carga en ejecución.")
            return

        print(f"Iniciando procesamiento para {len(file_paths)} archivos.")
        self.load_button.setEnabled(False)
        self.find_match_button.setEnabled(False)
        self.reprocess_button.setEnabled(False)
        self.status_label.setText("Iniciando procesamiento..."); self.progress_bar.setValue(0)

        self.processing_thread = QThread()
        self.processing_worker = ProcessingWorker(file_paths, self.api_client)
        self.processing_worker.moveToThread(self.processing_thread)
        self.processing_worker.progress_updated.connect(self.update_progress)
        self.processing_worker.file_processed.connect(self.update_document_table)
        self.processing_worker.error_occurred.connect(self.handle_processing_error)
        self.processing_worker.finished.connect(self.on_processing_finished)
        self.processing_thread.started.connect(self.processing_worker.run)
        self.processing_thread.start()

    def handle_reprocessing_error(self, doc_id, error_msg):
        print(f"Error reprocesando ID {doc_id}: {error_msg}")
        for row in range(self.table_widget.rowCount()):
            id_item = self.table_widget.item(row, 0)
            if id_item and id_item.text().isdigit() and int(id_item.text()) == doc_id:
                status_item = QTableWidgetItem("Error Reproceso")
                status_item.setBackground(Qt.red)
                self.table_widget.setItem(row, 2, status_item) # Columna Estado
                break

    def reprocess_selected(self):
        selected_rows = self.table_widget.selectionModel().selectedRows()
        if not selected_rows: return
        doc_ids_to_reprocess = []
        for index in selected_rows:
            id_item = self.table_widget.item(index.row(), 0)
            if id_item:
                try: doc_ids_to_reprocess.append(int(id_item.text()))
                except ValueError: print(f"ID inválido en fila {index.row()}: {id_item.text()}")
        if not doc_ids_to_reprocess: return

        if (self.processing_thread and self.processing_thread.isRunning()) or \
           (self.reprocessing_thread and self.reprocessing_thread.isRunning()):
            QMessageBox.warning(self, "Proceso en Curso", "Ya hay un proceso de carga o reproceso en ejecución.")
            return

        print(f"Iniciando reproceso para IDs: {doc_ids_to_reprocess}")
        self.load_button.setEnabled(False); self.find_match_button.setEnabled(False); self.reprocess_button.setEnabled(False)
        self.status_label.setText("Iniciando reprocesamiento..."); self.progress_bar.setValue(0)

        self.reprocessing_thread = QThread()
        self.reprocessing_worker = ReprocessingWorker(doc_ids_to_reprocess, self.api_client)
        self.reprocessing_worker.moveToThread(self.reprocessing_thread)
        self.reprocessing_worker.progress_updated.connect(self.update_progress)
        self.reprocessing_worker.file_processed.connect(self.update_document_table)
        self.reprocessing_worker.error_occurred.connect(self.handle_reprocessing_error)
        self.reprocessing_worker.finished.connect(self.on_reprocessing_finished)
        self.reprocessing_thread.started.connect(self.reprocessing_worker.run)
        self.reprocessing_thread.start()

    def on_reprocessing_finished(self):
        self.status_label.setText("Reproceso completado.")
        self.load_button.setEnabled(True)
        has_selection = self.table_widget.selectionModel().hasSelection()
        self.find_match_button.setEnabled(has_selection)
        self.reprocess_button.setEnabled(has_selection)
        if self.reprocessing_thread:
            self.reprocessing_thread.quit(); self.reprocessing_thread.wait()
        self.reprocessing_thread = None; self.reprocessing_worker = None
        print("Hilo de reprocesamiento terminado.")

    def on_selection_changed(self):
        selected_rows = self.table_widget.selectionModel().selectedRows()
        has_selection = bool(selected_rows)
        print(f"on_selection_changed llamado. Filas seleccionadas: {len(selected_rows)}, Habilitar botones: {has_selection}")
        is_processing = (self.processing_thread and self.processing_thread.isRunning()) or \
                        (self.reprocessing_thread and self.reprocessing_thread.isRunning())
        self.find_match_button.setEnabled(has_selection and not is_processing)
        self.reprocess_button.setEnabled(has_selection and not is_processing)

    def load_initial_documents(self):
        self.status_label.setText("Cargando documentos existentes...")
        QApplication.processEvents()
        try:
            documents = self.api_client.get_documents()
            if documents:
                self.table_widget.setRowCount(0)
                self.documents_data = {}
                print(f"Cargando {len(documents)} documentos existentes...")
                for doc_info in documents:
                    if 'filename' not in doc_info and 'file_url' in doc_info:
                         try: doc_info['filename'] = os.path.basename(doc_info['file_url'])
                         except: doc_info['filename'] = 'Nombre Desconocido'
                    elif 'filename' not in doc_info:
                         doc_info['filename'] = f"ID {doc_info.get('id', '?')}"

                    self.update_document_table(doc_info)
                self.status_label.setText(f"{len(documents)} documentos cargados. Puedes arrastrar archivos PDF a la tabla.")
            else:
                if isinstance(documents, dict) and 'error' in documents:
                     error_msg = documents['error']
                     print(f"Error API al cargar documentos iniciales: {error_msg}")
                     self.status_label.setText(f"Error API: {error_msg}")
                     QMessageBox.critical(self, "Error de Carga API", f"No se pudieron cargar los documentos:\n{error_msg}")
                else:
                     self.status_label.setText("No se encontraron documentos existentes.")
        except Exception as e:
            print(f"Error cargando documentos iniciales: {e}")
            traceback.print_exc()
            self.status_label.setText("Error al cargar documentos.")
            QMessageBox.critical(self, "Error de Carga", f"No se pudieron cargar los documentos:\n{e}")

    def update_progress(self, value, text):
        self.progress_bar.setValue(value); self.status_label.setText(text)

    def handle_item_changed(self, item):
        if self.is_saving_edit: # Evitar bucle si el cambio fue programático
            return

        row = item.row()
        col = item.column()

        # Solo actuar si se editó Fecha (4) o Importe (5)
        if col not in [4, 5]:
            return

        # Obtener ID del documento
        id_item = self.table_widget.item(row, 0)
        if not id_item or not id_item.text().isdigit():
            print(f"Error: No se pudo obtener ID válido en fila {row}")
            return
        doc_id = int(id_item.text())

        # Obtener datos actuales del documento localmente
        original_doc_info = self.documents_data.get(doc_id)
        if not original_doc_info:
             print(f"Error: No se encontraron datos locales para Doc ID {doc_id}")
             return # O intentar recargar desde API?

        # Determinar campo y valor
        new_value_str = item.text().strip()
        field_name = None
        original_value_str = "" # Para poder restaurar en caso de error

        # --- INICIO MODIFICACIÓN: Manejar None para extracted_data ---
        extracted_data_original = original_doc_info.get("extracted_data") # Obtener sin default primero

        if col == 4: # Fecha Factura
            field_name = "document_date"
            # Comprobar si extracted_data_original es None antes de usar .get()
            original_value_str = extracted_data_original.get("document_date", "") if extracted_data_original else ""
            original_value_str = original_value_str or "" # Asegurar que sea string si es None/vacío
        elif col == 5: # Importe Factura
            field_name = "total_amount"
            # Comprobar si extracted_data_original es None
            original_amount = extracted_data_original.get("total_amount") if extracted_data_original else None
            original_value_str = f"{float(original_amount):.2f}" if original_amount is not None else ""
        # --- FIN MODIFICACIÓN ---

        # Evitar llamada a API si el valor no cambió realmente
        if new_value_str == original_value_str:
            print(f"Valor no cambiado para Doc ID {doc_id}, Campo {field_name}. Ignorando.")
            return

        print(f"Item cambiado: Fila {row}, Col {col}, Doc ID {doc_id}, Campo: {field_name}, Nuevo Valor: '{new_value_str}'")

        # --- Llamada a la API para guardar ---
        self.is_saving_edit = True # Activar flag antes de la llamada
        self.status_label.setText(f"Guardando {field_name} para Doc ID {doc_id}...")
        QApplication.processEvents()

        response = self.api_client.update_extracted_data(doc_id, field_name, new_value_str)

        if response and isinstance(response, dict) and "id" in response and "error" not in response:
            # Éxito: API devuelve el documentInfo actualizado
            print(f"Guardado exitoso para Doc ID {doc_id}. Respuesta: {response}")
            self.status_label.setText(f"{field_name} para Doc ID {doc_id} guardado.")

            # Actualizar datos locales y la tabla completa para esa fila
            # La respuesta contiene el docInfo completo, así que podemos reusar update_document_table
            self.update_document_table(response)

        else:
            # Error
            error_msg = response.get("error", "Error desconocido al guardar") if isinstance(response, dict) else "Respuesta inválida de API"
            print(f"Error guardando {field_name} para Doc ID {doc_id}: {error_msg}")
            self.status_label.setText(f"Error al guardar Doc ID {doc_id}: {error_msg}")
            QMessageBox.warning(self, "Error al Guardar", f"No se pudo guardar el cambio para Doc ID {doc_id}:\n{error_msg}\n\nRestaurando valor original.")

            # Restaurar valor original en la celda
            item.setText(original_value_str)

        self.is_saving_edit = False

    def update_document_table(self, doc_info):
        """Actualiza o inserta una fila en la tabla con la información del documento."""
        doc_id = doc_info.get("id")
        if not doc_id:
            print("Error: doc_info recibido sin ID.")
            return

        # Si el doc_info viene de un proceso (create_with_ocr) puede no tener filename
        # pero sí lo tenemos guardado localmente si ya existía.
        if 'filename' not in doc_info:
            existing_data = self.documents_data.get(doc_id, {})
            doc_info['filename'] = existing_data.get('filename', f"ID {doc_id}")

        # Actualizar o añadir los datos locales
        self.documents_data[doc_id] = doc_info
        print(f"Actualizando tabla para Doc ID: {doc_id}, Info: {doc_info}")

        # Buscar si la fila ya existe
        row_index = -1
        for row in range(self.table_widget.rowCount()):
            id_item = self.table_widget.item(row, 0)
            if id_item and id_item.text().isdigit() and int(id_item.text()) == doc_id:
                row_index = row
                break

        # Si no existe, insertar una nueva fila
        if row_index == -1:
            row_index = self.table_widget.rowCount()
            self.table_widget.insertRow(row_index)
            # Columna 0: ID (oculta)
            id_item = QTableWidgetItem(str(doc_id))
            id_item.setFlags(id_item.flags() & ~Qt.ItemIsEditable) # No editable
            self.table_widget.setItem(row_index, 0, id_item)

        # --- Rellenar/Actualizar todas las celdas ---

        # Determinar si la fila es editable basado en el estado
        status_text = doc_info.get("get_status_display", doc_info.get("status", ""))
        # Define qué estados permiten la edición manual de Fecha/Importe
        editable_statuses = ['Necesita Mapeo Manual', 'Error en Procesamiento', 'Faltan Datos Extracción', 'Procesado', 'Error Fecha', 'Error Búsqueda'] # Ajusta según necesidad
        can_edit_row = status_text in editable_statuses

        # Columna 1: Archivo
        filename_item = QTableWidgetItem(doc_info.get("filename", ""))
        filename_item.setFlags(filename_item.flags() & ~Qt.ItemIsEditable) # No editable
        self.table_widget.setItem(row_index, 1, filename_item)

        # Columna 2: Estado
        status_item = QTableWidgetItem(status_text)
        status_item.setFlags(status_item.flags() & ~Qt.ItemIsEditable) # No editable
        self.table_widget.setItem(row_index, 2, status_item)

        # Columna 3: Tipo
        type_name = doc_info.get("document_type_name", "") or ""
        type_item = QTableWidgetItem(type_name)
        type_item.setFlags(type_item.flags() & ~Qt.ItemIsEditable) # No editable
        self.table_widget.setItem(row_index, 3, type_item)

        # Columna 4: Fecha Factura (Potencialmente editable)
        extracted_data = doc_info.get("extracted_data", {})
        document_date_str = extracted_data.get("document_date", "") if extracted_data else ""
        date_item = QTableWidgetItem(document_date_str or "")
        if can_edit_row:
            date_item.setFlags(date_item.flags() | Qt.ItemIsEditable) # Añadir flag editable
            date_item.setToolTip("Editable (Formato YYYY-MM-DD)")
        else:
            date_item.setFlags(date_item.flags() & ~Qt.ItemIsEditable) # Asegurar no editable
        self.table_widget.setItem(row_index, 4, date_item)

        # Columna 5: Importe Factura (Potencialmente editable)
        total_amount = extracted_data.get("total_amount") if extracted_data else None
        total_str = f"{float(total_amount):.2f}" if total_amount is not None else ""
        amount_item = QTableWidgetItem(total_str)
        if can_edit_row:
            amount_item.setFlags(amount_item.flags() | Qt.ItemIsEditable) # Añadir flag editable
            amount_item.setToolTip("Editable (Ej: 123.45)")
        else:
            amount_item.setFlags(amount_item.flags() & ~Qt.ItemIsEditable) # Asegurar no editable
        self.table_widget.setItem(row_index, 5, amount_item)

        # Columna 6: Cuenta Doc.
        account_id_from_type = doc_info.get("document_type_account_id", "")
        account_item = QTableWidgetItem(str(account_id_from_type) or "")
        account_item.setFlags(account_item.flags() & ~Qt.ItemIsEditable) # No editable
        self.table_widget.setItem(row_index, 6, account_item)

        # Columnas de Asiento (7 a 10 ahora) - Limpiar por defecto
        # <<<--- CAMBIO: Ajustar rango del bucle --->>>
        for col_idx in [7, 8, 9, 10]: # Antes era [7, 8, 9, 10, 11]
            item = QTableWidgetItem("")
            item.setFlags(item.flags() & ~Qt.ItemIsEditable)
            self.table_widget.setItem(row_index, col_idx, item)

        # Columna 11: Acción (Botones)
        # <<<--- CAMBIO: Índice de columna de acción --->>>
        existing_widget = self.table_widget.cellWidget(row_index, 11) # Antes era 12
        if existing_widget:
            # Si ya existe un widget (posiblemente de una actualización previa),
            # lo eliminamos para crear uno nuevo. deleteLater es más seguro.
            existing_widget.deleteLater()

        action_widget = QWidget()
        action_layout = QHBoxLayout(action_widget)
        action_layout.setContentsMargins(2, 2, 2, 2) # Márgenes pequeños
        action_layout.setSpacing(5) # Espacio entre botones

        # Botón Ver
        view_button = QPushButton("Ver")
        view_button.setToolTip("Abrir el archivo del documento")
        view_button.setProperty("doc_id", doc_id) # Guardar ID para el slot
        file_url = doc_info.get("file_url")
        if file_url:
            view_button.setProperty("file_url", file_url) # Guardar URL para el slot
            view_button.clicked.connect(self.on_view_button_clicked)
        else:
            view_button.setEnabled(False) # Deshabilitar si no hay URL
        action_layout.addWidget(view_button)

        # Botón Borrar
        delete_button = QPushButton("Borrar")
        delete_button.setToolTip("Eliminar este documento de la base de datos")
        delete_button.setProperty("doc_id", doc_id) # Guardar ID para el slot
        delete_button.clicked.connect(self.on_delete_button_clicked)
        action_layout.addWidget(delete_button)

        # Añadir un espacio flexible para empujar los botones a la izquierda
        action_layout.addStretch()

        # <<<--- CAMBIO: Índice de columna de acción --->>>
        self.table_widget.setCellWidget(row_index, 11, action_widget) # Antes era 12

        # --- Aplicar Coloreado ---
        self.apply_row_coloring(row_index, status_text)


    def apply_row_coloring(self, row_index, status_text):
        """Aplica color de fondo a una fila basado en el texto del estado."""
        color = None
        if status_text == 'Procesado': color = Qt.yellow
        elif status_text == 'Necesita Mapeo Manual': color = Qt.cyan
        elif status_text == 'Asociado a Transacción': color = Qt.green
        elif status_text in ['Error en Procesamiento', 'Error', 'Error Reproceso', 'Error Búsqueda', 'Error Fecha', 'Faltan Datos Extracción', 'Error API Búsqueda', 'Error Interno', 'Error Respuesta API']: color = Qt.red
        elif status_text == 'Coincidencia Encontrada': color = Qt.green
        elif status_text == 'Sin Coincidencia': color = Qt.lightGray

        base_color = color if color else Qt.white

        # Aplicar color a todas las columnas excepto la de acción
        # <<<--- CAMBIO: Ajustar rango del bucle --->>>
        for col in range(self.table_widget.columnCount() - 1): # columnCount() ahora es 12
            item = self.table_widget.item(row_index, col)
            if not item:
                item = QTableWidgetItem("")
                item.setFlags(item.flags() & ~Qt.ItemIsEditable)
                self.table_widget.setItem(row_index, col, item)
            item.setBackground(base_color)

    def find_matching_entries(self):
        selected_rows = self.table_widget.selectionModel().selectedRows()
        if not selected_rows: return
        tolerance_days = self.days_spinbox.value()
        self.status_label.setText(f"Buscando asientos (+/- {tolerance_days} días)...")
        QApplication.processEvents()

        processed_count = 0
        found_count = 0
        error_count = 0

        for index in selected_rows:
            row = index.row()
            id_item = self.table_widget.item(row, 0)
            if not id_item or not id_item.text().isdigit(): continue
            doc_id = int(id_item.text())
            doc_info = self.documents_data.get(doc_id)

            # --- Limpiar resultados previos en la fila (Columnas 7 a 10 ahora) ---
            # <<<--- CAMBIO: Ajustar rango del bucle --->>>
            for col_idx in [7, 8, 9, 10]: # Antes era [7, 8, 9, 10, 11]
                item = QTableWidgetItem("")
                item.setFlags(item.flags() & ~Qt.ItemIsEditable)
                self.table_widget.setItem(row, col_idx, item)
            # -------------------------------------------------------------

            # --- Recuperar el widget de acción existente ---
            # <<<--- CAMBIO: Índice de columna de acción --->>>
            action_widget = self.table_widget.cellWidget(row, 11) # Antes era 12
            action_layout = action_widget.layout() if action_widget else None
            if action_layout:
                for i in reversed(range(action_layout.count())):
                    item = action_layout.itemAt(i)
                    widget = item.widget()
                    if isinstance(widget, QPushButton) and widget.text() == "Asociar":
                        action_layout.takeAt(i)
                        widget.deleteLater()
                        print(f"Botón 'Asociar' previo eliminado para fila {row}")
                        break
            # ------------------------------------------------------------------------------------

            if not doc_info:
                print(f"No se encontraron datos internos para Doc ID: {doc_id}")
                status_item = QTableWidgetItem("Error Interno")
                self.table_widget.setItem(row, 2, status_item)
                self.apply_row_coloring(row, "Error Interno")
                error_count += 1
                continue

            account_id_to_match = doc_info.get("document_type_account_id")
            extracted_data = doc_info.get("extracted_data", {})
            document_date_str = extracted_data.get("document_date")
            total_amount_str = extracted_data.get("total_amount")
            document_total_amount = None

            if total_amount_str:
                try: document_total_amount = float(total_amount_str)
                except (ValueError, TypeError): pass

            print(f"Buscando para Doc ID {doc_id}: Cuenta={account_id_to_match}, Fecha={document_date_str}, Importe={document_total_amount}")

            if not account_id_to_match or not document_date_str or document_total_amount is None:
                print(f"Faltan datos para buscar coincidencias para doc {doc_id}")
                status_item = self.table_widget.item(row, 2)
                current_status_text = status_item.text() if status_item else ""
                allowed_statuses_to_overwrite = ["Procesado", "Necesita Mapeo Manual", "Pendiente de Procesar", ""]
                if current_status_text in allowed_statuses_to_overwrite:
                    new_status_item = QTableWidgetItem("Faltan Datos Extracción")
                    self.table_widget.setItem(row, 2, new_status_item)
                    self.apply_row_coloring(row, "Faltan Datos Extracción")
                error_count += 1
                continue

            try:
                from datetime import timedelta
                document_date = datetime.strptime(document_date_str, "%Y-%m-%d").date()
                start_date = (document_date - timedelta(days=tolerance_days)).strftime("%Y-%m-%d")
                end_date = (document_date + timedelta(days=tolerance_days)).strftime("%Y-%m-%d")
                print(f" -> Rango API: {start_date} a {end_date}")

                entries = self.api_client.get_accounting_entries(
                    account_id=account_id_to_match,
                    start_date=start_date,
                    end_date=end_date,
                    amount=document_total_amount
                )

                match_found = None
                if isinstance(entries, dict) and 'error' in entries:
                    print(f"Error API buscando asientos para doc {doc_id}: {entries['error']}")
                    status_item = QTableWidgetItem("Error API Búsqueda")
                    self.table_widget.setItem(row, 2, status_item)
                    self.apply_row_coloring(row, "Error API Búsqueda")
                    error_count += 1
                    continue

                if isinstance(entries, list): # Asegurarse que es una lista
                    if entries:
                        print(f" -> API devolvió {len(entries)} asientos candidatos.")
                        match_found = entries[0]
                        print(f" -> Coincidencia encontrada por API: Entry ID {match_found.get('id')}")
                        found_count += 1
                    else:
                        print(f" -> API devolvió 0 asientos tras filtrar.")
                else: # Si no es lista ni dict de error
                     print(f" -> Respuesta inesperada de API para doc {doc_id}: {entries}")
                     status_item = QTableWidgetItem("Error Respuesta API")
                     self.table_widget.setItem(row, 2, status_item)
                     self.apply_row_coloring(row, "Error Respuesta API")
                     error_count += 1
                     continue

                # --- Actualizar fila con resultado ---
                current_status_text = ""
                if match_found:
                    current_status_text = "Coincidencia Encontrada"
                    status_item = QTableWidgetItem(current_status_text)
                    self.table_widget.setItem(row, 2, status_item)

                    entry_id_str = str(match_found.get("id", ""))
                    self.table_widget.setItem(row, 7, QTableWidgetItem(entry_id_str))

                    # Columna 8: Fecha Asiento
                    entry_date_str = match_found.get("date", "")
                    self.table_widget.setItem(row, 8, QTableWidgetItem(entry_date_str))

                    # <<<--- CAMBIO: Bloque de Importe Asiento ELIMINADO --->>>

                    # Columna 9: Desc. Asiento (Antes 10)
                    # <<<--- CAMBIO: Índice de columna --->>>
                    self.table_widget.setItem(row, 9, QTableWidgetItem(match_found.get("description", "")))

                    # Columna 10: Cuentas Asiento (Antes 11)
                    # <<<--- CAMBIO: Índice de columna --->>>
                    accounts_in_entry = []
                    for trans in match_found.get("transactions", []):
                        acc_name = trans.get("account_name", f"ID:{trans.get('account', '?')}")
                        debit = trans.get("debit", 0)
                        credit = trans.get("credit", 0)
                        amount_str = f"{float(debit):.2f}" if debit else f"{float(credit):.2f}"
                        accounts_in_entry.append(f"{acc_name} ({amount_str})")
                    accounts_str = "; ".join(accounts_in_entry)
                    self.table_widget.setItem(row, 10, QTableWidgetItem(accounts_str))

                    # --- Añadir botón "Asociar" al layout existente ---
                    if action_layout:
                        associate_button = QPushButton("Asociar")
                        associate_button.setToolTip(f"Asociar Doc {doc_id} con Asiento {match_found['id']}")
                        associate_button.setProperty("doc_id", doc_id)
                        associate_button.setProperty("entry_id", match_found['id'])
                        associate_button.clicked.connect(self.on_associate_button_clicked)

                        stretch_index = -1
                        for i in range(action_layout.count()):
                            item = action_layout.itemAt(i)
                            if item and item.spacerItem():
                                stretch_index = i
                                break
                        if stretch_index != -1:
                            action_layout.insertWidget(stretch_index, associate_button)
                            print(f"Botón 'Asociar' insertado en índice {stretch_index} para fila {row}")
                        else:
                            action_layout.addWidget(associate_button)
                            print(f"Botón 'Asociar' añadido al final para fila {row}")
                    else:
                        # <<<--- CAMBIO: Índice de columna en mensaje de advertencia (no aplica directamente) --->>>
                        print(f"Advertencia: No se encontró action_layout para fila {row}, no se pudo añadir botón 'Asociar'.")
                    # -------------------------------------------------

                else: # No se encontró coincidencia
                    current_status_text = "Sin Coincidencia"
                    print(f" -> No se encontraron coincidencias para doc {doc_id}")
                    status_item = QTableWidgetItem(current_status_text)
                    self.table_widget.setItem(row, 2, status_item)

                processed_count += 1
                self.apply_row_coloring(row, current_status_text)

            except ValueError as ve:
                print(f"Error parseando fecha '{document_date_str}' para doc {doc_id}: {ve}")
                status_item = QTableWidgetItem("Error Fecha")
                self.table_widget.setItem(row, 2, status_item)
                self.apply_row_coloring(row, "Error Fecha")
                error_count += 1
            except Exception as e:
                print(f"Error buscando coincidencias para doc {doc_id}: {e}")
                traceback.print_exc()
                status_item = QTableWidgetItem("Error Búsqueda")
                self.table_widget.setItem(row, 2, status_item)
                self.apply_row_coloring(row, "Error Búsqueda")
                error_count += 1

            final_status_item = self.table_widget.item(row, 2)
            if final_status_item and doc_id in self.documents_data:
                 self.documents_data[doc_id]['status'] = final_status_item.text()

        self.status_label.setText(f"Búsqueda completada. Procesados: {processed_count}, Encontrados: {found_count}, Errores: {error_count}")

    def on_delete_button_clicked(self):
        button = self.sender()
        if not button: return
        doc_id = button.property("doc_id")
        if doc_id is None: return

        doc_info = self.documents_data.get(doc_id)
        filename = doc_info.get('filename', f'ID {doc_id}') if doc_info else f'ID {doc_id}'

        reply = QMessageBox.question(self, 'Confirmar Borrado',
                                    f"¿Estás seguro de que quieres eliminar permanentemente el documento '{filename}' (ID: {doc_id})?\n\nEsta acción no se puede deshacer.",
                                    QMessageBox.Yes | QMessageBox.No, QMessageBox.No)

        if reply == QMessageBox.Yes:
            print(f"Intentando borrar Doc ID: {doc_id}")

            target_row = -1
            for row in range(self.table_widget.rowCount()):
                id_item = self.table_widget.item(row, 0)
                if id_item and id_item.text().isdigit() and int(id_item.text()) == doc_id:
                    target_row = row
                    break

            if target_row == -1:
                print(f"Error: No se encontró la fila para Doc ID {doc_id} en la tabla.")
                QMessageBox.warning(self, "Error Interno", f"No se pudo encontrar la fila para el documento {doc_id}.")
                return

            self.status_label.setText(f"Borrando documento {doc_id}...")
            QApplication.processEvents()

            # --- Deshabilitar botones en la fila encontrada ---
            # <<<--- CAMBIO: Índice de columna de acción --->>>
            action_widget = self.table_widget.cellWidget(target_row, 11) # Antes era 12
            buttons_to_disable = []
            if action_widget:
                buttons_to_disable = action_widget.findChildren(QPushButton)
                for btn in buttons_to_disable:
                    btn.setEnabled(False)
            # -------------------------------------------------

            response = self.api_client.delete_document(doc_id)

            if response is True:
                print(f"Documento {doc_id} borrado exitosamente.")
                self.status_label.setText(f"Documento {doc_id} eliminado.")
                self.table_widget.removeRow(target_row)
                if doc_id in self.documents_data:
                    del self.documents_data[doc_id]
            else:
                error_msg = "Error desconocido al borrar"
                if isinstance(response, dict) and "error" in response:
                    error_msg = response["error"]
                elif response is False:
                     error_msg = "Fallo en la petición API (ver consola)."
                elif response is None:
                     error_msg = "No se recibió respuesta válida de la API."

                print(f"Error al borrar Doc ID {doc_id}: {error_msg}")
                QMessageBox.critical(self, "Error de Borrado", f"No se pudo eliminar el documento {doc_id}:\n{error_msg}")
                self.status_label.setText(f"Error al borrar doc {doc_id}.")

                for btn in buttons_to_disable:
                     btn.setEnabled(True)

    def on_view_button_clicked(self):
        button = self.sender()
        if not button: return
        file_url = button.property("file_url")
        doc_id = button.property("doc_id")

        if file_url:
            print(f"Intentando abrir archivo para Doc ID {doc_id}: {file_url}")
            qurl_to_open = QUrl(file_url)
            if not qurl_to_open.isValid() or qurl_to_open.scheme() == "":
                 qurl_to_open = QUrl.fromLocalFile(file_url)

            print(f"  -> Abriendo QUrl: {qurl_to_open.toString()}")
            success = QDesktopServices.openUrl(qurl_to_open)
            if not success:
                QMessageBox.warning(self, "Error al Abrir", f"No se pudo abrir el archivo o URL:\n{file_url}\n\nAsegúrate de tener un programa asociado o que la ruta sea accesible.")
                print(f"Fallo al abrir URL: {file_url}")
        else:
            QMessageBox.information(self, "Sin Archivo", f"No hay URL de archivo asociada al documento ID {doc_id}.")
            print(f"No file_url para Doc ID {doc_id}")

    def on_associate_button_clicked(self):
        button = self.sender()
        if not button: return
        doc_id = button.property("doc_id")
        entry_id = button.property("entry_id")
        if doc_id is None or entry_id is None: return

        print(f"Intentando FINALIZAR adjunto para Doc ID: {doc_id} a Asiento ID: {entry_id}")
        self.status_label.setText(f"Finalizando doc {doc_id}...")
        QApplication.processEvents()
        button.setEnabled(False)

        response = self.api_client.finalize_document_attachment(doc_id, entry_id)

        if response and isinstance(response, dict) and "error" not in response and response.get("status") == "ATTACHMENT_CREATED_DOC_DELETED":
            print(f"Documento {doc_id} finalizado (adjuntado a asiento {entry_id} y eliminado).")
            self.status_label.setText(f"Documento {doc_id} finalizado.")

            target_row = -1
            for row in range(self.table_widget.rowCount()):
                id_item = self.table_widget.item(row, 0)
                if id_item and id_item.text().isdigit() and int(id_item.text()) == doc_id:
                    target_row = row
                    break
            if target_row != -1:
                 self.table_widget.removeRow(target_row)

            if doc_id in self.documents_data:
                del self.documents_data[doc_id]

        else:
            error_msg = response.get("error", "Error desconocido al finalizar") if isinstance(response, dict) else "Respuesta inválida de API"
            print(f"Error al finalizar Doc ID {doc_id}: {error_msg}")
            QMessageBox.warning(self, "Error Finalización", f"No se pudo finalizar el documento {doc_id}:\n{error_msg}")
            self.status_label.setText(f"Error al finalizar doc {doc_id}.")
            button.setEnabled(True)

    def handle_processing_error(self, filename, error_msg):
        print(f"Error procesando {filename}: {error_msg}")
        error_data = {
            "id": f"error_{filename}",
            "filename": filename,
            "status": "Error",
            "get_status_display": "Error"
        }
        self.update_document_table(error_data)
        for row in range(self.table_widget.rowCount()):
             file_item = self.table_widget.item(row, 1)
             if file_item and file_item.text() == filename:
                 status_item = self.table_widget.item(row, 2)
                 if status_item and status_item.text() == "Error":
                      self.apply_row_coloring(row, "Error")
                      break

    def on_processing_finished(self):
        self.status_label.setText("Proceso de carga y OCR completado.")
        self.load_button.setEnabled(True)
        self.on_selection_changed()
        if self.processing_thread:
            self.processing_thread.quit(); self.processing_thread.wait()
        self.processing_thread = None; self.processing_worker = None
        print("Hilo de procesamiento terminado.")

    def closeEvent(self, event):
        if self.processing_worker: self.processing_worker.stop()
        if self.processing_thread and self.processing_thread.isRunning(): self.processing_thread.quit(); self.processing_thread.wait()
        if self.reprocessing_worker: self.reprocessing_worker.stop()
        if self.reprocessing_thread and self.reprocessing_thread.isRunning(): self.reprocessing_thread.quit(); self.reprocessing_thread.wait()
        event.accept()

# --- Bloque Principal ---
if __name__ == '__main__':
    app = QApplication(sys.argv)
    # --- Añadir imports locales aquí si es necesario ---
    # from garca_api_client import ApiClient
    # from bulk_matcher_ProcessWoker import ProcessingWorker, ReprocessingWorker
    # -------------------------------------------------
    main_window = MainWindow()
    main_window.show()
    sys.exit(app.exec_())
