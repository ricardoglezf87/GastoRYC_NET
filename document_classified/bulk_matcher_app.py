# bulk_matcher_app.py

import sys
import os
import traceback
from PyQt5.QtWidgets import (
    QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout,
    QPushButton, QTableWidget, QTableWidgetItem, QHeaderView,
    QProgressBar, QLabel, QFileDialog, QSpinBox, QAbstractItemView,
    QMessageBox
)
# Asegúrate de importar QUrl
from PyQt5.QtCore import Qt, QThread, QUrl, pyqtSignal 
from PyQt5.QtGui import QDesktopServices
from .garca_api_client import ApiClient

from .bulk_matcher_ProcessWoker import ProcessingWorker, ReprocessingWorker

class DropTableWidget(QTableWidget):
    filesDropped = pyqtSignal(list)

    def __init__(self, parent=None):
        super().__init__(parent)
        self.setAcceptDrops(True)
        print("DropTableWidget: __init__ - Drops habilitados.") # DEBUG

    def dragEnterEvent(self, event):
        print("DropTableWidget: dragEnterEvent triggered") # DEBUG
        mime_data = event.mimeData()
        print(f"  Mime formats: {mime_data.formats()}") # DEBUG
        print(f"  Mime URLs: {mime_data.urls()}") # DEBUG

        if mime_data.hasUrls():
            # Comprobación simplificada temporalmente: Aceptar cualquier archivo
            print("  Tiene URLs. Aceptando acción propuesta.") # DEBUG
            event.acceptProposedAction()
            self.setStyleSheet("background-color: #e0f0ff;") # Azul claro para feedback
            # --- Comentamos la comprobación de PDF temporalmente ---
            # has_pdf = False
            # for url in mime_data.urls():
            #     if url.isLocalFile() and url.toLocalFile().lower().endswith('.pdf'):
            #         has_pdf = True
            #         break
            # if has_pdf:
            #     print("  Tiene PDF. Aceptando acción propuesta.") # DEBUG
            #     event.acceptProposedAction()
            #     self.setStyleSheet("background-color: #e0f0ff;") # Azul claro
            # else:
            #     print("  No tiene PDF. Ignorando.") # DEBUG
            #     event.ignore()
            # -----------------------------------------------------
        else:
            print("  No tiene URLs. Ignorando.") # DEBUG
            event.ignore()

    def dragMoveEvent(self, event):
        # Este evento es crucial. Si no se acepta aquí, el cursor puede seguir prohibido.
        print("DropTableWidget: dragMoveEvent triggered") # DEBUG
        if event.mimeData().hasUrls():
            # print("  dragMove: Aceptando") # DEBUG (puede ser muy verboso)
            event.acceptProposedAction()
        else:
            # print("  dragMove: Ignorando") # DEBUG
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
                    # Volvemos a poner la comprobación de PDF aquí, en el drop final
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
        self.setGeometry(100, 100, 1400, 700) # Ajustar tamaño si es necesario

        self.api_client = ApiClient()
        self.documents_data = {}
        self.processing_thread = None
        self.processing_worker = None
        self.reprocessing_thread = None # Añadir atributo
        self.reprocessing_worker = None # Añadir atributo

        self.init_ui()
        self.load_initial_documents()

    def init_ui(self):
        central_widget = QWidget()
        self.setCentralWidget(central_widget)
        main_layout = QVBoxLayout(central_widget)

        # --- Sección Superior: Controles ---
        controls_layout = QHBoxLayout()
        self.load_button = QPushButton("Cargar documentos PDF")
        self.load_button.clicked.connect(self.load_files_dialog) # Renombrar para claridad
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
        self.table_widget.setColumnCount(11)
        self.table_widget.setHorizontalHeaderLabels([
            "ID", "Archivo", "Estado", "Tipo", "Fecha Factura",
            "Importe Factura", "Cuenta Doc.", "ID Asiento", "Desc. Asiento", "Cuentas Asiento", "Acción"
        ])
        self.table_widget.horizontalHeader().setSectionResizeMode(1, QHeaderView.Stretch)
        self.table_widget.horizontalHeader().setSectionResizeMode(2, QHeaderView.ResizeToContents)
        self.table_widget.horizontalHeader().setSectionResizeMode(8, QHeaderView.Stretch)
        self.table_widget.horizontalHeader().setSectionResizeMode(9, QHeaderView.Stretch)
        self.table_widget.horizontalHeader().setSectionResizeMode(10, QHeaderView.ResizeToContents)
        self.table_widget.setColumnHidden(0, True)
        self.table_widget.setSelectionBehavior(QAbstractItemView.SelectRows)
        self.table_widget.itemSelectionChanged.connect(self.on_selection_changed)
        self.table_widget.cellClicked.connect(self.on_selection_changed)

        # <<<--- CAMBIO 1: Habilitar Drag and Drop en la tabla --->>>
        self.table_widget.setAcceptDrops(True)
        # <<<----------------------------------------------------->>>

        main_layout.addWidget(self.table_widget)
        # --- FIN Sección Media ---

        # --- Sección Inferior: Progreso y Estado ---
        status_layout = QHBoxLayout()
        self.progress_bar = QProgressBar()
        status_layout.addWidget(self.progress_bar)
        self.status_label = QLabel("Listo. Puedes arrastrar archivos PDF a la tabla.") # Mensaje actualizado
        status_layout.addWidget(self.status_label)
        main_layout.addLayout(status_layout)
        # --- FIN Sección Inferior ---

    # --- Slots y Métodos ---

    # <<<--- CAMBIO 3: Renombrar y refactorizar la carga de archivos --->>>
    def load_files_dialog(self):
        """Abre el diálogo para seleccionar archivos."""
        options = QFileDialog.Options()
        files, _ = QFileDialog.getOpenFileNames(self,"Seleccionar documentos PDF","", "Archivos PDF (*.pdf);;Todos los archivos (*)", options=options)
        if files:
            self.start_processing_files(files) # Llamar a la nueva función
        else:
            print("Selección de archivo cancelada.")

    def start_processing_files(self, file_paths):
        """Inicia el hilo de procesamiento para una lista de rutas de archivo."""
        if not file_paths:
            return

        # Evitar iniciar si ya está procesando
        if self.processing_thread and self.processing_thread.isRunning():
            QMessageBox.warning(self, "Proceso en Curso", "Ya hay un proceso de carga en ejecución.")
            return

        print(f"Iniciando procesamiento para {len(file_paths)} archivos.")
        self.load_button.setEnabled(False)
        self.find_match_button.setEnabled(False) # Deshabilitar mientras procesa
        self.reprocess_button.setEnabled(False) # Deshabilitar mientras procesa
        self.status_label.setText("Iniciando procesamiento..."); self.progress_bar.setValue(0)

        # Crear e iniciar el hilo (como ya lo hacías)
        self.processing_thread = QThread()
        self.processing_worker = ProcessingWorker(file_paths, self.api_client)
        self.processing_worker.moveToThread(self.processing_thread)
        self.processing_worker.progress_updated.connect(self.update_progress)
        self.processing_worker.file_processed.connect(self.update_document_table)
        self.processing_worker.error_occurred.connect(self.handle_processing_error)
        self.processing_worker.finished.connect(self.on_processing_finished)
        self.processing_thread.started.connect(self.processing_worker.run)
        self.processing_thread.start()
    # <<<------------------------------------------------------------->>>

    def handle_reprocessing_error(self, doc_id, error_msg):
        # ... (código sin cambios) ...
        print(f"Error reprocesando ID {doc_id}: {error_msg}")
        for row in range(self.table_widget.rowCount()):
            id_item = self.table_widget.item(row, 0)
            if id_item and int(id_item.text()) == doc_id:
                status_item = QTableWidgetItem("Error Reproceso")
                status_item.setBackground(Qt.red)
                self.table_widget.setItem(row, 2, status_item) # Columna Estado
                break

    def reprocess_selected(self):
        # ... (código sin cambios) ...
        selected_rows = self.table_widget.selectionModel().selectedRows()
        if not selected_rows: return
        doc_ids_to_reprocess = []
        for index in selected_rows:
            id_item = self.table_widget.item(index.row(), 0)
            if id_item:
                try: doc_ids_to_reprocess.append(int(id_item.text()))
                except ValueError: print(f"ID inválido en fila {index.row()}: {id_item.text()}")
        if not doc_ids_to_reprocess: return

        # Evitar iniciar si ya está procesando/reprocesando
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
        # ... (código sin cambios) ...
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
        # ... (código sin cambios) ...
        selected_rows = self.table_widget.selectionModel().selectedRows()
        has_selection = bool(selected_rows)
        print(f"on_selection_changed llamado. Filas seleccionadas: {len(selected_rows)}, Habilitar botones: {has_selection}")
        # Solo habilitar si no hay un proceso en curso
        is_processing = (self.processing_thread and self.processing_thread.isRunning()) or \
                        (self.reprocessing_thread and self.reprocessing_thread.isRunning())
        self.find_match_button.setEnabled(has_selection and not is_processing)
        self.reprocess_button.setEnabled(has_selection and not is_processing)


    def load_initial_documents(self):
        # ... (código sin cambios) ...
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
                    self.update_document_table(doc_info)
                self.status_label.setText(f"{len(documents)} documentos cargados. Puedes arrastrar archivos PDF a la tabla.") # Mensaje actualizado
            else:
                self.status_label.setText("No se encontraron documentos existentes o hubo un error.")
        except Exception as e:
            print(f"Error cargando documentos iniciales: {e}")
            traceback.print_exc()
            self.status_label.setText("Error al cargar documentos.")
            QMessageBox.critical(self, "Error de Carga", f"No se pudieron cargar los documentos:\n{e}")

    # --- load_files ahora es load_files_dialog ---

    def update_progress(self, value, text):
        # ... (código sin cambios) ...
        self.progress_bar.setValue(value); self.status_label.setText(text)

    def update_document_table(self, doc_info):
        # ... (código sin cambios) ...
        doc_id = doc_info.get("id")
        if not doc_id: print("Error: doc_info recibido sin ID."); return

        if 'filename' not in doc_info:
            existing_data = self.documents_data.get(doc_id, {})
            doc_info['filename'] = existing_data.get('filename', f"ID {doc_id}")

        self.documents_data[doc_id] = doc_info
        print(f"Actualizando tabla para Doc ID: {doc_id}, Info: {doc_info}")

        row_index = -1
        for row in range(self.table_widget.rowCount()):
            id_item = self.table_widget.item(row, 0)
            if id_item and int(id_item.text()) == doc_id: row_index = row; break
        if row_index == -1:
            row_index = self.table_widget.rowCount()
            self.table_widget.insertRow(row_index)
            self.table_widget.setItem(row_index, 0, QTableWidgetItem(str(doc_id)))

        # Llenar celdas
        self.table_widget.setItem(row_index, 1, QTableWidgetItem(doc_info.get("filename", "")))
        self.table_widget.setItem(row_index, 2, QTableWidgetItem(doc_info.get("get_status_display", doc_info.get("status", ""))))
        self.table_widget.setItem(row_index, 3, QTableWidgetItem(doc_info.get("document_type_name", "") or ""))
        extracted_data = doc_info.get("extracted_data", {})
        document_date_str = extracted_data.get("document_date", "") if extracted_data else ""
        self.table_widget.setItem(row_index, 4, QTableWidgetItem(document_date_str or ""))
        total_amount = extracted_data.get("total_amount") if extracted_data else None
        total_str = f"{float(total_amount):.2f}" if total_amount is not None else ""
        self.table_widget.setItem(row_index, 5, QTableWidgetItem(total_str))
        account_id_from_type = doc_info.get("document_type_account_id", "")
        self.table_widget.setItem(row_index, 6, QTableWidgetItem(str(account_id_from_type) or ""))
        self.table_widget.setItem(row_index, 7, QTableWidgetItem("")) # ID Asiento
        self.table_widget.setItem(row_index, 8, QTableWidgetItem("")) # Desc. Asiento
        self.table_widget.setItem(row_index, 9, QTableWidgetItem("")) # Cuentas Asiento
        self.table_widget.removeCellWidget(row_index, 10) # Asegurar que no haya botón inicialmente

        action_widget = QWidget()
        action_layout = QHBoxLayout(action_widget)
        action_layout.setContentsMargins(2, 2, 2, 2) # Márgenes pequeños
        action_layout.setSpacing(5)

        # --- Botón Ver ---
        view_button = QPushButton("Ver")
        view_button.setToolTip("Abrir el archivo del documento")
        view_button.setProperty("doc_id", doc_id)
        file_url = doc_info.get("file_url")
        if file_url:
            view_button.setProperty("file_url", file_url)
            view_button.clicked.connect(self.on_view_button_clicked) # Conectar a nuevo slot
        else:
            view_button.setEnabled(False) # Deshabilitar si no hay URL
        action_layout.addWidget(view_button)

        # --- Botón Borrar ---
        delete_button = QPushButton("Borrar")
        delete_button.setToolTip("Eliminar este documento de la base de datos")
        delete_button.setProperty("doc_id", doc_id)
        delete_button.clicked.connect(self.on_delete_button_clicked) # Conectar a nuevo slot
        action_layout.addWidget(delete_button)

        action_layout.addStretch() # Empuja los botones a la izquierda
        self.table_widget.setCellWidget(row_index, 10, action_widget) # Establecer el widget en la celda

        # Coloreado
        status = doc_info.get("status", "")
        color = None
        if status == 'PROCESSED': color = Qt.yellow
        elif status == 'NEEDS_MAPPING': color = Qt.cyan
        elif status == 'ASSOCIATED': color = Qt.green
        elif status in ['FAILED', 'Error', 'Error Reproceso']: color = Qt.red # Incluir Error Reproceso
        if color:
            for col in range(self.table_widget.columnCount()):
                item = self.table_widget.item(row_index, col)
                if not item: item = QTableWidgetItem(""); self.table_widget.setItem(row_index, col, item)
                item.setBackground(color)
        else: # Limpiar color si no aplica
             for col in range(self.table_widget.columnCount()):
                item = self.table_widget.item(row_index, col)
                if item: item.setBackground(Qt.white) # O el color por defecto

    def find_matching_entries(self):
        # ... (código sin cambios) ...
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
            if not id_item or not id_item.text().isdigit(): continue # Saltar si no hay ID válido
            doc_id = int(id_item.text())
            doc_info = self.documents_data.get(doc_id)

            # --- Limpiar resultados previos en la fila ---
            self.table_widget.setItem(row, 7, QTableWidgetItem("")) # ID Asiento
            self.table_widget.setItem(row, 8, QTableWidgetItem("")) # Desc. Asiento
            self.table_widget.setItem(row, 9, QTableWidgetItem("")) # Cuentas Asiento
            # Recuperar el widget de acción existente para añadir/quitar el botón "Asociar"
            action_widget = self.table_widget.cellWidget(row, 10)
            action_layout = action_widget.layout() if action_widget else None
            # Eliminar botón "Asociar" si existía previamente
            if action_layout:
                for i in reversed(range(action_layout.count())):
                    item = action_layout.itemAt(i)
                    widget = item.widget()
                    # Comprobar si es un QPushButton y su texto es "Asociar"
                    if isinstance(widget, QPushButton) and widget.text() == "Asociar":
                        # Eliminar el widget del layout y luego borrarlo
                        action_layout.takeAt(i) # Quita el item del layout
                        widget.deleteLater()    # Programa la eliminación del widget
                        print(f"Botón 'Asociar' previo eliminado para fila {row}")
                        break # Asumimos solo un botón "Asociar" por fila
            # -------------------------------------------

            if not doc_info:
                print(f"No se encontraron datos internos para Doc ID: {doc_id}")
                status_item = QTableWidgetItem("Error Interno")
                status_item.setBackground(Qt.red)
                self.table_widget.setItem(row, 2, status_item)
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
                # Solo actualizar si no es un error más grave o ya asociado
                if status_item and status_item.text() not in ["Error Búsqueda", "Asociado", "Error Interno", "Error Fecha", "FAILED", "Error Reproceso"]:
                    new_status_item = QTableWidgetItem("Faltan Datos Extracción")
                    new_status_item.setBackground(Qt.red) # O un color específico
                    self.table_widget.setItem(row, 2, new_status_item)
                error_count += 1
                continue

            try:
                from datetime import datetime, timedelta
                document_date = datetime.strptime(document_date_str, "%Y-%m-%d").date()
                start_date = (document_date - timedelta(days=tolerance_days)).strftime("%Y-%m-%d")
                end_date = (document_date + timedelta(days=tolerance_days)).strftime("%Y-%m-%d")
                print(f" -> Rango API: {start_date} a {end_date}")

                # --- LLAMADA API ---
                # Pasar el importe para que el backend filtre si es posible
                entries = self.api_client.get_accounting_entries(
                    account_id=account_id_to_match,
                    start_date=start_date,
                    end_date=end_date,
                    amount=document_total_amount
                )
                # -------------------

                match_found = None
                # Si la API devuelve error, 'entries' será un dict con 'error'
                if isinstance(entries, dict) and 'error' in entries:
                    print(f"Error API buscando asientos para doc {doc_id}: {entries['error']}")
                    status_item = QTableWidgetItem("Error API Búsqueda")
                    status_item.setBackground(Qt.red)
                    self.table_widget.setItem(row, 2, status_item)
                    error_count += 1
                    continue # Pasar al siguiente documento

                # Si la API devuelve una lista (puede estar vacía)
                if entries:
                    print(f" -> API devolvió {len(entries)} asientos candidatos.")
                    # Asumimos que la API ya filtró por importe y devuelve la mejor/única coincidencia
                    # Si devuelve múltiples, podrías necesitar lógica adicional aquí para elegir.
                    if len(entries) > 0:
                        match_found = entries[0] # Tomamos el primero
                        print(f" -> Coincidencia encontrada por API: Entry ID {match_found.get('id')}")
                        found_count += 1
                    else:
                        print(f" -> API devolvió 0 asientos tras filtrar.")

                # --- Actualizar fila con resultado ---
                if match_found:
                    status_item = QTableWidgetItem("Coincidencia Encontrada")
                    status_item.setBackground(Qt.yellow) # Color para coincidencia
                    self.table_widget.setItem(row, 2, status_item)
                    self.table_widget.setItem(row, 7, QTableWidgetItem(str(match_found.get("id", ""))))
                    self.table_widget.setItem(row, 8, QTableWidgetItem(match_found.get("description", "")))

                    # Construir string de cuentas del asiento encontrado
                    accounts_in_entry = []
                    for trans in match_found.get("transactions", []):
                        acc_name = trans.get("account_name", f"ID:{trans.get('account', '?')}")
                        debit = trans.get("debit", 0)
                        credit = trans.get("credit", 0)
                        # Mostrar el importe relevante de la transacción
                        amount_str = f"{float(debit):.2f}" if debit else f"{float(credit):.2f}"
                        accounts_in_entry.append(f"{acc_name} ({amount_str})")
                    accounts_str = "; ".join(accounts_in_entry)
                    self.table_widget.setItem(row, 9, QTableWidgetItem(accounts_str))

                    # --- Añadir botón "Asociar" al layout existente ---
                    if action_layout:
                        associate_button = QPushButton("Asociar")
                        associate_button.setToolTip(f"Asociar Doc {doc_id} con Asiento {match_found['id']}")
                        associate_button.setProperty("doc_id", doc_id)
                        associate_button.setProperty("entry_id", match_found['id'])
                        associate_button.clicked.connect(self.on_associate_button_clicked)

                        # Insertar antes del stretch item (que suele ser el último)
                        stretch_index = -1
                        for i in range(action_layout.count()):
                            # Comprobar si es un QSpacerItem (el stretch)
                            item = action_layout.itemAt(i)
                            if item and item.spacerItem(): # Método más fiable para detectar stretch
                                stretch_index = i
                                break
                        if stretch_index != -1:
                            action_layout.insertWidget(stretch_index, associate_button)
                            print(f"Botón 'Asociar' insertado en índice {stretch_index} para fila {row}")
                        else:
                            action_layout.addWidget(associate_button) # Añadir al final si no hay stretch
                            print(f"Botón 'Asociar' añadido al final para fila {row}")
                    else:
                        print(f"Advertencia: No se encontró action_layout para fila {row}, no se pudo añadir botón 'Asociar'.")
                    # -------------------------------------------------

                else: # No se encontró coincidencia
                    print(f" -> No se encontraron coincidencias para doc {doc_id}")
                    status_item = QTableWidgetItem("Sin Coincidencia")
                    status_item.setBackground(Qt.lightGray) # Color para sin coincidencia
                    self.table_widget.setItem(row, 2, status_item)
                    # Las celdas 7, 8, 9 ya se limpiaron al principio
                    # El botón "Asociar" ya se eliminó al principio

                processed_count += 1

            except ValueError as ve:
                print(f"Error parseando fecha '{document_date_str}' para doc {doc_id}: {ve}")
                status_item = QTableWidgetItem("Error Fecha")
                status_item.setBackground(Qt.red)
                self.table_widget.setItem(row, 2, status_item)
                error_count += 1
            except Exception as e:
                print(f"Error buscando coincidencias para doc {doc_id}: {e}")
                traceback.print_exc()
                status_item = QTableWidgetItem("Error Búsqueda")
                status_item.setBackground(Qt.red)
                self.table_widget.setItem(row, 2, status_item)
                error_count += 1

            # --- Actualizar colores y layout de la fila después de procesar ---
            # Recuperar la info actualizada (puede haber cambiado el estado)
            current_doc_info = self.documents_data.get(doc_id)
            if current_doc_info:
                # Actualizar el estado en la info interna si cambió en la tabla
                current_status_item = self.table_widget.item(row, 2)
                if current_status_item:
                    current_doc_info['status'] = current_status_item.text()

                # --- Aplicar coloreado basado en el estado actual ---
                status_val = current_doc_info.get("status", "")
                color = None
                if status_val == 'PROCESSED': color = Qt.yellow
                elif status_val == 'NEEDS_MAPPING': color = Qt.cyan
                elif status_val == 'ASSOCIATED': color = Qt.green
                elif status_val in ['FAILED', 'Error', 'Error Reproceso', 'Error Búsqueda', 'Error Fecha', 'Faltan Datos Extracción', 'Error API Búsqueda', 'Error Interno']: color = Qt.red
                elif status_val == 'Coincidencia Encontrada': color = Qt.yellow # O un verde claro?
                elif status_val == 'Sin Coincidencia': color = Qt.lightGray

                base_color = color if color else Qt.white # Color base

                for col in range(self.table_widget.columnCount() -1): # Excluir columna de acción
                    item = self.table_widget.item(row, col)
                    if not item: # Crear item si no existe para poder colorear
                        item = QTableWidgetItem("")
                        item.setFlags(item.flags() & ~Qt.ItemIsEditable)
                        self.table_widget.setItem(row, col, item)
                    item.setBackground(base_color)
                # ----------------------------------------------------

            # ----------------------------------------------------------------

        self.status_label.setText(f"Búsqueda completada. Procesados: {processed_count}, Encontrados: {found_count}, Errores: {error_count}")


    def on_delete_button_clicked(self):
        # ... (código sin cambios) ...
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

            # --- Encontrar la fila correspondiente al doc_id ---
            target_row = -1
            for row in range(self.table_widget.rowCount()):
                id_item = self.table_widget.item(row, 0) # Columna 0 (ID oculto)
                if id_item and id_item.text().isdigit() and int(id_item.text()) == doc_id:
                    target_row = row
                    break
            # ----------------------------------------------------

            if target_row == -1:
                print(f"Error: No se encontró la fila para Doc ID {doc_id} en la tabla.")
                QMessageBox.warning(self, "Error Interno", f"No se pudo encontrar la fila para el documento {doc_id}.")
                return

            self.status_label.setText(f"Borrando documento {doc_id}...")
            QApplication.processEvents()

            # --- Deshabilitar botones en la fila encontrada ---
            action_widget = self.table_widget.cellWidget(target_row, 10) # Columna 10 (Acción)
            buttons_to_disable = []
            if action_widget:
                buttons_to_disable = action_widget.findChildren(QPushButton)
                for btn in buttons_to_disable:
                    btn.setEnabled(False)
            # -------------------------------------------------

            # --- LLAMAR AL MÉTODO API REAL ---
            response = self.api_client.delete_document(doc_id)
            # ---------------------------------

            # Verificar la respuesta (esperamos True si fue exitoso - 204 No Content)
            if response is True:
                print(f"Documento {doc_id} borrado exitosamente.")
                self.status_label.setText(f"Documento {doc_id} eliminado.")

                # --- Eliminar la fila encontrada de la tabla ---
                self.table_widget.removeRow(target_row)
                # ---------------------------------------------

                # Eliminar de datos internos
                if doc_id in self.documents_data:
                    del self.documents_data[doc_id]

            else: # Falló la operación en el backend o hubo otro error
                error_msg = "Error desconocido al borrar"
                if isinstance(response, dict) and "error" in response:
                    error_msg = response["error"]
                elif response is False: # Podría ser un error genérico del _make_request
                     error_msg = "Fallo en la petición API (ver consola)."
                elif response is None:
                     error_msg = "No se recibió respuesta válida de la API."

                print(f"Error al borrar Doc ID {doc_id}: {error_msg}")
                QMessageBox.critical(self, "Error de Borrado", f"No se pudo eliminar el documento {doc_id}:\n{error_msg}")
                self.status_label.setText(f"Error al borrar doc {doc_id}.")

                # --- Volver a habilitar botones si falló ---
                # (No es necesario buscar la fila de nuevo, ya tenemos 'buttons_to_disable')
                for btn in buttons_to_disable:
                     # Podrías añadir lógica extra si algún botón no debe rehabilitarse siempre
                     btn.setEnabled(True)

    def on_view_button_clicked(self):
        # ... (código sin cambios) ...
        button = self.sender()
        if not button: return
        file_url = button.property("file_url")
        doc_id = button.property("doc_id")

        if file_url:
            print(f"Intentando abrir archivo para Doc ID {doc_id}: {file_url}")
            # Usar QDesktopServices para abrir la URL (puede ser local o http)
            success = QDesktopServices.openUrl(QUrl(file_url))
            if not success:
                QMessageBox.warning(self, "Error al Abrir", f"No se pudo abrir el archivo:\n{file_url}\n\nAsegúrate de tener un programa asociado.")
                print(f"Fallo al abrir URL: {file_url}")
        else:
            QMessageBox.information(self, "Sin Archivo", f"No hay URL de archivo asociada al documento ID {doc_id}.")
            print(f"No file_url para Doc ID {doc_id}")

    def on_associate_button_clicked(self):
        # ... (código sin cambios) ...
        button = self.sender()
        if not button: return
        doc_id = button.property("doc_id")
        entry_id = button.property("entry_id")
        if doc_id is None or entry_id is None: return

        print(f"Intentando FINALIZAR adjunto para Doc ID: {doc_id} a Asiento ID: {entry_id}")
        self.status_label.setText(f"Finalizando doc {doc_id}...")
        QApplication.processEvents()
        button.setEnabled(False)

        # --- LLAMAR AL NUEVO MÉTODO API ---
        response = self.api_client.finalize_document_attachment(doc_id, entry_id)
        # ---------------------------------

        # Verificar la respuesta del backend
        if response and isinstance(response, dict) and "error" not in response and response.get("status") == "ATTACHMENT_CREATED_DOC_DELETED":
            print(f"Documento {doc_id} finalizado (adjuntado a asiento {entry_id} y eliminado).")
            self.status_label.setText(f"Documento {doc_id} finalizado.")

            # Actualizar UI (Eliminar fila)
            for row in range(self.table_widget.rowCount()):
                id_item = self.table_widget.item(row, 0)
                if id_item and int(id_item.text()) == doc_id:
                    self.table_widget.removeRow(row)
                    break
            if doc_id in self.documents_data:
                del self.documents_data[doc_id]

        else: # Falló la operación en el backend
            error_msg = response.get("error", "Error desconocido al finalizar") if isinstance(response, dict) else "Respuesta inválida de API"
            print(f"Error al finalizar Doc ID {doc_id}: {error_msg}")
            QMessageBox.warning(self, "Error Finalización", f"No se pudo finalizar el documento {doc_id}:\n{error_msg}")
            self.status_label.setText(f"Error al finalizar doc {doc_id}.")
            button.setEnabled(True) # Habilitar botón de nuevo si falló

    def handle_processing_error(self, filename, error_msg):
        # ... (código sin cambios) ...
        print(f"Error procesando {filename}: {error_msg}")
        row_index = self.table_widget.rowCount()
        self.table_widget.insertRow(row_index)
        self.table_widget.setItem(row_index, 0, QTableWidgetItem("-1"))
        self.table_widget.setItem(row_index, 1, QTableWidgetItem(filename))
        status_item = QTableWidgetItem("Error")
        status_item.setBackground(Qt.red)
        self.table_widget.setItem(row_index, 2, status_item)

    def on_processing_finished(self):
        # ... (código sin cambios) ...
        self.status_label.setText("Proceso de carga y OCR completado.")
        self.load_button.setEnabled(True)
        # Habilitar botones de acción si hay selección
        self.on_selection_changed()
        if self.processing_thread:
            self.processing_thread.quit(); self.processing_thread.wait()
        self.processing_thread = None; self.processing_worker = None
        print("Hilo de procesamiento terminado.")

    def closeEvent(self, event):
        # ... (código sin cambios) ...
        if self.processing_worker: self.processing_worker.stop()
        if self.processing_thread and self.processing_thread.isRunning(): self.processing_thread.quit(); self.processing_thread.wait()
        if self.reprocessing_worker: self.reprocessing_worker.stop()
        if self.reprocessing_thread and self.reprocessing_thread.isRunning(): self.reprocessing_thread.quit(); self.reprocessing_thread.wait()
        event.accept()

# --- Bloque Principal ---
if __name__ == '__main__':
    app = QApplication(sys.argv)
    main_window = MainWindow()
    main_window.show()
    sys.exit(app.exec_())
