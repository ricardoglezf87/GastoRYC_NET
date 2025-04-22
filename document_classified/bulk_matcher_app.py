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
from PyQt5.QtCore import Qt, QThread
from .garca_api_client import ApiClient

from .bulk_matcher_ProcessWoker import ProcessingWorker, ReprocessingWorker



# --- Ventana Principal ---
class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Conciliador de Documentos Masivo")
        self.setGeometry(100, 100, 1200, 700) # Ajustar tamaño si es necesario

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
        self.load_button.clicked.connect(self.load_files)
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
        self.table_widget = QTableWidget()
        # --- CORREGIR NÚMERO DE COLUMNAS ---
        self.table_widget.setColumnCount(11) # ID(oculto), Archivo, Estado, Tipo, FechaFac, ImpFac, CuentaFac, ID Asiento, Desc Asiento, Cuentas Asiento, Acción
        # ------------------------------------
        self.table_widget.setHorizontalHeaderLabels([
            "ID", "Archivo", "Estado", "Tipo", "Fecha Factura",
            "Importe Factura", "Cuenta Doc.", "ID Asiento", "Desc. Asiento", "Cuentas Asiento", "Acción"
        ])
        self.table_widget.horizontalHeader().setSectionResizeMode(1, QHeaderView.Stretch) # Archivo
        self.table_widget.horizontalHeader().setSectionResizeMode(2, QHeaderView.ResizeToContents) # Estado
        self.table_widget.horizontalHeader().setSectionResizeMode(8, QHeaderView.Stretch) # Desc. Asiento
        self.table_widget.horizontalHeader().setSectionResizeMode(9, QHeaderView.Stretch) # Cuentas Asiento
        self.table_widget.setColumnHidden(0, True)
        self.table_widget.setSelectionBehavior(QAbstractItemView.SelectRows)
        self.table_widget.itemSelectionChanged.connect(self.on_selection_changed)
        self.table_widget.cellClicked.connect(self.on_selection_changed)
        main_layout.addWidget(self.table_widget)
        # --- FIN Sección Media ---

        # --- Sección Inferior: Progreso y Estado ---
        status_layout = QHBoxLayout()
        self.progress_bar = QProgressBar()
        status_layout.addWidget(self.progress_bar)
        self.status_label = QLabel("Listo.")
        status_layout.addWidget(self.status_label)
        main_layout.addLayout(status_layout)
        # --- FIN Sección Inferior ---

    # --- Slots y Métodos ---
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
        self.find_match_button.setEnabled(has_selection)
        self.reprocess_button.setEnabled(has_selection)

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
                    self.update_document_table(doc_info)
                self.status_label.setText(f"{len(documents)} documentos cargados.")
            else:
                self.status_label.setText("No se encontraron documentos existentes o hubo un error.")
        except Exception as e:
            print(f"Error cargando documentos iniciales: {e}")
            traceback.print_exc()
            self.status_label.setText("Error al cargar documentos.")
            # --- USAR QMessageBox ---
            QMessageBox.critical(self, "Error de Carga", f"No se pudieron cargar los documentos:\n{e}")
            # -----------------------

    def load_files(self):
        # ... (código sin cambios) ...
        options = QFileDialog.Options()
        files, _ = QFileDialog.getOpenFileNames(self,"Seleccionar documentos PDF","", "Archivos PDF (*.pdf);;Todos los archivos (*)", options=options)
        if files:
            print(f"Archivos seleccionados: {len(files)}")
            self.load_button.setEnabled(False)
            self.status_label.setText("Iniciando procesamiento..."); self.progress_bar.setValue(0)
            self.processing_thread = QThread()
            self.processing_worker = ProcessingWorker(files, self.api_client)
            self.processing_worker.moveToThread(self.processing_thread)
            self.processing_worker.progress_updated.connect(self.update_progress)
            self.processing_worker.file_processed.connect(self.update_document_table)
            self.processing_worker.error_occurred.connect(self.handle_processing_error)
            self.processing_worker.finished.connect(self.on_processing_finished)
            self.processing_thread.started.connect(self.processing_worker.run)
            self.processing_thread.start()
        else: print("Selección de archivo cancelada.")

    def update_progress(self, value, text):
        # ... (código sin cambios) ...
        self.progress_bar.setValue(value); self.status_label.setText(text)

    def update_document_table(self, doc_info):
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
        # --- CORREGIR DUPLICACIÓN ---
        account_id_from_type = doc_info.get("document_type_account_id", "")
        self.table_widget.setItem(row_index, 6, QTableWidgetItem(str(account_id_from_type) or ""))
        # --- FIN CORRECCIÓN ---
        self.table_widget.setItem(row_index, 7, QTableWidgetItem("")) # ID Asiento
        self.table_widget.setItem(row_index, 8, QTableWidgetItem("")) # Desc. Asiento
        self.table_widget.setItem(row_index, 9, QTableWidgetItem("")) # Cuentas Asiento
        self.table_widget.removeCellWidget(row_index, 10) # Asegurar que no haya botón inicialmente

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
        # ... (código sin cambios, asumiendo que está correcto) ...
        selected_rows = self.table_widget.selectionModel().selectedRows()
        if not selected_rows: return
        tolerance_days = self.days_spinbox.value()
        self.status_label.setText(f"Buscando asientos (+/- {tolerance_days} días)...")
        QApplication.processEvents()

        for index in selected_rows:
            row = index.row()
            id_item = self.table_widget.item(row, 0)
            if not id_item: continue
            doc_id = int(id_item.text())
            doc_info = self.documents_data.get(doc_id)
            if not doc_info: print(f"No se encontraron datos internos para Doc ID: {doc_id}"); continue

            account_id_to_match = doc_info.get("document_type_account_id")
            extracted_data = doc_info.get("extracted_data", {})
            document_date_str = extracted_data.get("document_date")
            total_amount_str = extracted_data.get("total_amount")
            document_total_amount = None
            if total_amount_str:
                try: document_total_amount = float(total_amount_str)
                except (ValueError, TypeError): pass

            print(f"DEBUG: Doc ID {doc_id} - Importe Factura a buscar: {document_total_amount}")

            if not account_id_to_match or not document_date_str or document_total_amount is None:
                 print(f"Faltan datos (cuenta={account_id_to_match}, fecha={document_date_str}, importe={document_total_amount}) para buscar coincidencias para doc {doc_id}")
                 status_item = self.table_widget.item(row, 2)
                 if status_item and status_item.text() not in ["Error Búsqueda", "Asociado"]:
                     self.table_widget.setItem(row, 2, QTableWidgetItem("Faltan Datos Extracción"))
                 continue

            try:
                from datetime import datetime, timedelta
                document_date = datetime.strptime(document_date_str, "%Y-%m-%d").date()
                start_date = (document_date - timedelta(days=tolerance_days)).strftime("%Y-%m-%d")
                end_date = (document_date + timedelta(days=tolerance_days)).strftime("%Y-%m-%d")
                print(f"Buscando asientos entre {start_date} y {end_date} para doc {doc_id}")

                entries = self.api_client.get_accounting_entries(account_id_to_match, start_date, end_date)

                match_found = None
                relevant_entry_amount = 0.0

                if entries:
                    print(f"DEBUG: Entries encontradas: {len(entries)}")
                    for entry in entries:
                        entry_transactions = entry.get("transactions", [])
                        for transaction in entry_transactions:
                            print(f"DEBUG:   Transacción {transaction.get('id')} - Cuenta: {transaction.get('account')} (Buscando: {account_id_to_match})")
                            if transaction.get("account") == account_id_to_match:
                                try:
                                    debit = float(transaction.get("debit", 0))
                                    credit = float(transaction.get("credit", 0))
                                    current_entry_amount = debit if debit != 0 else credit
                                    print(f"DEBUG:     Comparando: Asiento={current_entry_amount} vs Factura={document_total_amount}")
                                    if abs(current_entry_amount - document_total_amount) < 0.01:
                                        match_found = entry
                                        relevant_entry_amount = current_entry_amount
                                        print(f"Coincidencia encontrada para doc {doc_id}: Entry ID {entry['id']}, Transaction ID {transaction['id']}, Amount {relevant_entry_amount}")
                                        break
                                except (ValueError, TypeError) as e:
                                     print(f"Error convirtiendo debit/credit: {e}")
                                     continue
                        if match_found: break

                if match_found:
                    self.table_widget.setItem(row, 2, QTableWidgetItem("Coincidencia Encontrada"))
                    self.table_widget.setItem(row, 7, QTableWidgetItem(str(match_found.get("id", ""))))
                    self.table_widget.setItem(row, 8, QTableWidgetItem(match_found.get("description", "")))
                    accounts_in_entry = []
                    for trans in match_found.get("transactions", []):
                        acc_name = trans.get("account_name")
                        if not acc_name: acc_name = f"ID:{trans.get('account', '?')}"
                        accounts_in_entry.append(acc_name)
                    accounts_str = "; ".join(accounts_in_entry)
                    self.table_widget.setItem(row, 9, QTableWidgetItem(accounts_str))
                    associate_button = QPushButton("Asociar")
                    associate_button.setProperty("doc_id", doc_id)
                    associate_button.setProperty("entry_id", match_found['id'])
                    associate_button.clicked.connect(self.on_associate_button_clicked)
                    self.table_widget.setCellWidget(row, 10, associate_button)
                else:
                    print(f"No se encontraron coincidencias para doc {doc_id}")
                    self.table_widget.setItem(row, 2, QTableWidgetItem("Sin Coincidencia"))
                    self.table_widget.setItem(row, 7, QTableWidgetItem(""))
                    self.table_widget.setItem(row, 8, QTableWidgetItem(""))
                    self.table_widget.setItem(row, 9, QTableWidgetItem(""))
                    self.table_widget.removeCellWidget(row, 10)

            except ValueError as ve:
                 print(f"Error parseando fecha '{document_date_str}' para doc {doc_id}: {ve}")
                 self.table_widget.setItem(row, 2, QTableWidgetItem("Error Fecha"))
            except Exception as e:
                 print(f"Error buscando coincidencias para doc {doc_id}: {e}")
                 traceback.print_exc()
                 self.table_widget.setItem(row, 2, QTableWidgetItem("Error Búsqueda"))

        self.status_label.setText("Búsqueda de asientos completada.")

    # --- SOLO UNA DEFINICIÓN DE on_associate_button_clicked ---
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

    # --- ELIMINAR LA SEGUNDA DEFINICIÓN DE on_associate_button_clicked ---

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
        if self.processing_thread:
            self.processing_thread.quit(); self.processing_thread.wait()
        self.processing_thread = None; self.processing_worker = None
        print("Hilo de procesamiento terminado.")

    # --- ELIMINAR LA SEGUNDA DEFINICIÓN DE on_processing_finished ---

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
