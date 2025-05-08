# types_manager_app.py
import django
import sys
import os
import traceback # Para imprimir errores detallados
from PyQt5.QtWidgets import (QApplication, QWidget, QVBoxLayout, QLabel,
                             QLineEdit, QPushButton, QComboBox, QMessageBox,
                             QTextEdit,QFileDialog)
from PyQt5.QtCore import Qt, pyqtSlot, QStringListModel, QUrl
from PyQt5.QtWidgets import QCompleter

from .garca_api_client import ApiClient

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'GARCA.settings')
try:
    django.setup()
    # print("DJANGO SETUP: Django inicializado correctamente.")
except Exception as e:
    print(f"DJANGO SETUP ERROR: No se pudo inicializar Django: {e}")

from .processing_logic import extract_document_data, perform_ocr

class MappingWindow(QWidget):
    def __init__(self, document_id, initial_file_path, api_client, parent=None):
        super().__init__(parent)
        self.document_id = document_id
        self.file_path = initial_file_path
        self.api_client = api_client
        self.current_document_type_id = None
        self.document_types_data = {}

        self.setWindowTitle("Gestor de Tipos de Factura - Editor de Texto")
        self.layout = QVBoxLayout(self)
        self.setMinimumSize(700, 800)

        self.setAcceptDrops(True)
        
        # --- Sección de Selección de Archivo ---
        self.file_select_button = QPushButton("Seleccionar Archivo PDF")
        self.file_select_button.clicked.connect(self.select_file)
        self.layout.addWidget(self.file_select_button)

        # Mensaje inicial actualizado para incluir drag and drop
        self.selected_file_label = QLabel("Archivo: Ninguno (o arrastra un PDF aquí)")
        self.layout.addWidget(self.selected_file_label)
        # --------------------------------------------

        # --- Selección de Tipo (sin cambios) ---
        self.layout.addWidget(QLabel("Tipo de Factura:"))
        self.type_combo = QComboBox()
        self.type_combo.addItem("-- Crear Nuevo Tipo --", userData=None)
        self.layout.addWidget(self.type_combo)
        self.type_combo.currentIndexChanged.connect(self.on_type_selected)

        # --- Visor de Texto OCR (sin cambios) ---
        self.layout.addWidget(QLabel("Texto Extraído (OCR):"))
        self.text_edit = QTextEdit()
        self.text_edit.setReadOnly(True)
        self.text_edit.setLineWrapMode(QTextEdit.NoWrap)
        self.text_edit.setFontFamily("Courier New")
        self.layout.addWidget(self.text_edit, 1)

        # --- Campos de Reglas (sin cambios) ---
        self.layout.addWidget(QLabel("Nombre del Tipo:"))
        self.name_input = QLineEdit()
        self.layout.addWidget(self.name_input)
        self.layout.addWidget(QLabel("Identificador (Palabra Clave):"))
        self.identifier_keyword_input = QLineEdit()
        self.layout.addWidget(self.identifier_keyword_input)
        self.layout.addWidget(QLabel("Regla Fecha (Regex):"))
        self.date_regex_input = QLineEdit()
        self.layout.addWidget(self.date_regex_input)
        self.layout.addWidget(QLabel("Regla Total (Regex):"))
        self.total_regex_input = QLineEdit()
        self.layout.addWidget(self.total_regex_input)

        # --- Selección de Cuenta (sin cambios) ---
        self.layout.addWidget(QLabel("Cuenta Contable (Filtrable):"))
        self.account_combo = QComboBox()
        self.account_combo.setEditable(True)
        self.account_combo.setInsertPolicy(QComboBox.NoInsert)
        self.account_combo.setPlaceholderText("Escribe para filtrar...")
        self.account_completer = QCompleter(self)
        self.account_completer.setFilterMode(Qt.MatchContains)
        self.account_completer.setCompletionMode(QCompleter.PopupCompletion)
        self.account_completer.setCaseSensitivity(Qt.CaseInsensitive)
        self.account_combo.setCompleter(self.account_completer)
        self.layout.addWidget(self.account_combo)

        # --- Botones (sin cambios) ---
        self.save_button = QPushButton("Guardar Tipo")
        self.save_button.clicked.connect(self.save_type)
        self.layout.addWidget(self.save_button)
        self.test_button = QPushButton("Probar Reglas con Texto Actual")
        self.test_button.clicked.connect(self.test_template_rules)
        self.layout.addWidget(self.test_button)

        # Actualizar label si se pasó una ruta inicial
        if self.file_path:
             self.selected_file_label.setText(f"Archivo: {os.path.basename(self.file_path)}")

        self.load_document_types()
        self.load_accounts()

    def dragEnterEvent(self, event):
        mime_data = event.mimeData()
        # Aceptar solo si contiene URLs y al menos una es un archivo PDF local
        if mime_data.hasUrls():
            has_pdf = False
            for url in mime_data.urls():
                if url.isLocalFile() and url.toLocalFile().lower().endswith('.pdf'):
                    has_pdf = True
                    break
            if has_pdf:
                event.acceptProposedAction()
                # Opcional: Cambiar estilo para feedback visual
                self.setStyleSheet("background-color: #e0f0ff;")
            else:
                event.ignore() # Ignorar si no hay PDFs
        else:
            event.ignore()

    def dragMoveEvent(self, event):
        # Necesario para que el drop funcione correctamente
        if event.mimeData().hasUrls():
             event.acceptProposedAction()
        else:
            event.ignore()

    def dragLeaveEvent(self, event):
        # Restaurar estilo original
        self.setStyleSheet("")
        event.accept()

    def dropEvent(self, event):
        # Restaurar estilo original
        self.setStyleSheet("")
        mime_data = event.mimeData()

        if mime_data.hasUrls():
            urls = mime_data.urls()
            # Aceptar solo si es UN único archivo PDF local
            if len(urls) == 1 and urls[0].isLocalFile():
                file_path = urls[0].toLocalFile()
                if os.path.isfile(file_path) and file_path.lower().endswith('.pdf'):
                    event.acceptProposedAction()
                    self.file_path = file_path
                    self.selected_file_label.setText(f"Archivo: {os.path.basename(self.file_path)}")
                    self.load_document_text() # Cargar y hacer OCR
                else:
                    event.ignore()
            else:
                event.ignore()
        else:
            event.ignore()
            
    def select_file(self):
        """Abre un diálogo para seleccionar un archivo PDF."""
        options = QFileDialog.Options()
        file_path, _ = QFileDialog.getOpenFileName(
            self,
            "Seleccionar Factura PDF",
            "",
            "Archivos PDF (*.pdf);;Todos los archivos (*)",
            options=options
        )

        if file_path:
            self.file_path = file_path
            # Actualiza el label para mostrar el nombre del archivo
            self.selected_file_label.setText(f"Archivo: {os.path.basename(self.file_path)}")
            self.load_document_text()

    def load_document_text(self):
        """Realiza OCR en el PDF (self.file_path) y muestra el texto."""
        if not self.file_path or not os.path.exists(self.file_path):
             error_msg = "Por favor, selecciona un archivo PDF válido primero (o arrástralo aquí)."
             self.text_edit.setPlainText(error_msg)
             # Actualizar también la etiqueta del archivo
             self.selected_file_label.setText("Archivo: Ninguno (o arrastra un PDF aquí)")
             return

        self.text_edit.setPlainText("Realizando OCR, por favor espera...") # Mensaje temporal
        QApplication.processEvents() # Forzar actualización de la UI

        try:
            extracted_text = perform_ocr(self.file_path)
            if extracted_text is not None:
                self.text_edit.setPlainText(extracted_text)
            else:
                error_msg = "Error durante el OCR. Revisa la consola para más detalles."
                self.text_edit.setPlainText(error_msg)
        except Exception as e:
             error_msg = f"Error inesperado al cargar texto OCR: {e.__class__.__name__}: {e}"
             traceback.print_exc()
             self.text_edit.setPlainText(error_msg)
             QMessageBox.critical(self, "Error Crítico OCR", error_msg)

    def load_document_types(self):
        """Carga los tipos de factura desde la API y los añade al ComboBox."""
        self.type_combo.blockSignals(True)
        while self.type_combo.count() > 1:
            self.type_combo.removeItem(1)
        self.document_types_data = {}

        types = self.api_client.get_document_types()
        if types:
            try:
                types_sorted = sorted(types, key=lambda x: str(x.get('name', '')).lower())
            except TypeError:
                print("load_document_types: Advertencia - No se pudo ordenar los tipos por nombre.")
                types_sorted = types

            for inv_type in types_sorted:
                type_id = inv_type.get('id')
                type_name = inv_type.get('name')
                if type_id is not None and type_name:
                    self.type_combo.addItem(type_name, userData=type_id)
                    self.document_types_data[type_id] = inv_type
        self.type_combo.blockSignals(False)

    @pyqtSlot(int)
    def on_type_selected(self, index):
        """Se ejecuta cuando el usuario selecciona un item en el ComboBox de tipos."""
        selected_id = self.type_combo.itemData(index)

        if selected_id is None:
            self.current_document_type_id = None
            self.clear_type_fields()
        else:
            self.current_document_type_id = selected_id
            type_data = self.document_types_data.get(selected_id)
            if type_data and 'extraction_rules' in type_data:
                 self.populate_type_fields(type_data)
            else:
                 # print(f"on_type_selected: Obteniendo detalles de API para tipo ID: {selected_id}")
                 type_details = self.api_client.get_document_type(selected_id)
                 if type_details:
                     self.document_types_data[selected_id] = type_details
                     self.populate_type_fields(type_details)
                 else:
                     error_detail = f"No se pudo obtener detalles para el tipo ID {selected_id}."
                     print(f"Error cargando detalles: {error_detail}")
                     QMessageBox.warning(self, "Error", error_detail)
                     self.clear_type_fields()

    def clear_type_fields(self):
        """Limpia los campos de entrada de reglas."""
        self.name_input.clear()
        self.identifier_keyword_input.clear()
        self.date_regex_input.clear()
        self.total_regex_input.clear()
        # Resetear ComboBox de cuenta a "-- Sin asignar --"
        self.account_combo.setCurrentIndex(0)

    def populate_type_fields(self, type_data):
        """Rellena los campos de entrada con los datos de un documentType."""
        self.clear_type_fields()
        self.name_input.setText(type_data.get('name', ''))
        rules = type_data.get('extraction_rules', {})
        if not isinstance(rules, dict): rules = {}

        identifier_rule = rules.get('identifier', {})
        if identifier_rule.get('type') == 'keyword':
            self.identifier_keyword_input.setText(identifier_rule.get('value', ''))

        date_rule = rules.get('date', {})
        if date_rule.get('type') == 'regex':
            self.date_regex_input.setText(date_rule.get('pattern', ''))

        total_rule = rules.get('total', {})
        if total_rule.get('type') == 'regex':
            self.total_regex_input.setText(total_rule.get('pattern', ''))

        account_id = type_data.get('account')
        if account_id is not None:
            index = self.account_combo.findData(account_id)
            if index >= 0:
                self.account_combo.setCurrentIndex(index)
            else:
                print(f"Advertencia: Cuenta ID {account_id} del tipo no encontrada en el ComboBox.")
                self.account_combo.setCurrentIndex(0)
        else:
            self.account_combo.setCurrentIndex(0)

    def load_accounts(self):
        """Carga las cuentas contables desde la API y las añade al ComboBox."""
        self.account_combo.clear()
        self.account_combo.setEnabled(False)
        account_display_names = []

        try:
            accounts = self.api_client.get_accounts()
            if accounts:
                try:
                    accounts_sorted = sorted(accounts, key=lambda x: str(x.get('get_full_hierarchy', x.get('name', ''))))
                except TypeError:
                    print("load_accounts: Advertencia: No se pudo ordenar las cuentas.") # DEBUG
                    accounts_sorted = accounts

                added_count = 0
                self.account_combo.addItem("-- Sin asignar --", userData=None)
                account_display_names.append("-- Sin asignar --")

                for acc in accounts_sorted:
                    acc_id = acc.get('id')
                    display_name = acc.get('get_full_hierarchy', acc.get('name'))
                    if acc_id is not None and display_name:
                        self.account_combo.addItem(display_name, userData=acc_id)
                        account_display_names.append(display_name)
                        added_count += 1

                if added_count > 0:
                    self.account_combo.setEnabled(True)
                else:
                    self.account_combo.clear()
                    self.account_combo.addItem("No hay cuentas disponibles", userData=None)
                    account_display_names = ["No hay cuentas disponibles"]

            else:
                self.account_combo.addItem("Error al cargar cuentas", userData=None)
                account_display_names = ["Error al cargar cuentas"]

            self.account_model = QStringListModel(account_display_names, self)
            self.account_completer.setModel(self.account_model)

        except Exception as e:
            traceback.print_exc()
            self.account_combo.clear()
            self.account_combo.addItem("Error crítico al cargar cuentas", userData=None)
            self.account_combo.setEnabled(False)

    def _build_rules_payload(self):
        """Construye el diccionario de reglas a partir de los campos de entrada."""
        rules = {}
        identifier_val = self.identifier_keyword_input.text().strip()
        date_val = self.date_regex_input.text().strip()
        total_val = self.total_regex_input.text().strip()

        if identifier_val: rules["identifier"] = {"type": "keyword", "value": identifier_val}
        if date_val: rules["date"] = {"type": "regex", "pattern": date_val}
        if total_val: rules["total"] = {"type": "regex", "pattern": total_val}

        return rules

    def save_type(self):
        """Guarda el tipo actual (crea uno nuevo o actualiza uno existente)."""
        rules = self._build_rules_payload()
        name = self.name_input.text().strip()
        account_id = self.account_combo.currentData()

        if not name:
            QMessageBox.warning(self, "Faltan Datos", "Introduce un nombre para el tipo.")
            return

        payload = {
            "name": name,
            "extraction_rules": rules,
            "account": account_id
        }

        result = None
        if self.current_document_type_id is None:
            result = self.api_client.create_document_type(payload)

            if isinstance(result, dict) and 'id' in result and "error" not in result:
                 new_id = result['id']
                 QMessageBox.information(self, "Éxito", f"Tipo '{name}' (ID: {new_id}) creado correctamente.")
                 self.load_document_types()
                 index = self.type_combo.findData(new_id)
                 if index >= 0:
                     self.type_combo.setCurrentIndex(index)
            else:
                 error_msg = self._format_api_error(result, "crear")
                 QMessageBox.critical(self, "Error API", error_msg)

        else:
            result = self.api_client.update_document_type(self.current_document_type_id, payload)

            if isinstance(result, dict) and 'id' in result and "error" not in result:
                 updated_id = result['id']
                 QMessageBox.information(self, "Éxito", f"Tipo '{name}' (ID: {updated_id}) actualizado correctamente.")
                 self.document_types_data[updated_id] = result
                 current_index = self.type_combo.currentIndex()
                 if self.type_combo.itemData(current_index) == updated_id and self.type_combo.itemText(current_index) != name:
                      self.type_combo.setItemText(current_index, name)
            else:
                 error_msg = self._format_api_error(result, "actualizar")
                 QMessageBox.critical(self, "Error API", error_msg)

    def _format_api_error(self, result, action="guardar"):
        """Formatea un mensaje de error a partir de la respuesta de la API."""
        error_msg = f"Error desconocido al {action} el tipo."
        if isinstance(result, dict):
            if "error" in result:
                error_msg = result['error']
            else:
                try:
                    error_details = "\n".join([f"- {k}: {v[0] if isinstance(v, list) else v}" for k, v in result.items()])
                    if error_details:
                         error_msg = f"No se pudo {action} el tipo debido a errores:\n{error_details}"
                    elif 'non_field_errors' in result:
                         error_msg = f"Error al {action}: {result['non_field_errors'][0]}"
                except Exception as parse_err:
                     print(f"Error parseando error de API: {parse_err}")
                     error_msg = f"Respuesta de error inesperada del servidor: {result}"
        elif result is None:
            error_msg = f"No se recibió respuesta del servidor al intentar {action}."
        else:
            error_msg = str(result)
        return f"No se pudo {action} el tipo:\n{error_msg}"

    def test_template_rules(self):
        """Prueba las reglas actuales contra el texto mostrado en el QTextEdit."""
        rules = self._build_rules_payload()
        if not rules:
             QMessageBox.warning(self, "Sin Reglas", "Define al menos una regla para probar.")
             return

        current_text = self.text_edit.toPlainText()

        if not current_text or "Realizando OCR" in current_text or "Error" in current_text:
             QMessageBox.warning(self, "Sin Texto", "No hay texto OCR válido cargado para probar.")
             return

        extracted_data = extract_document_data(current_text, rules)

        result_text = "Resultado de la Prueba Local:\n\n"
        if not extracted_data:
             result_text += "No se extrajo ningún dato con las reglas proporcionadas."
        else:
            for key, value in extracted_data.items():
                field_name = key.replace('_', ' ').title()
                if value is None:
                    field_value = "No encontrado"
                elif isinstance(value, (float, int)):
                    field_value = f"{value:.2f}"
                else:
                    field_value = str(value)
                result_text += f"{field_name}: {field_value}\n"
        QMessageBox.information(self, "Resultado Prueba Local", result_text.strip())

if __name__ == '__main__':
   app = QApplication.instance()
if app is None:
    app = QApplication(sys.argv)

api = ApiClient()

try:
    mapper = MappingWindow(document_id=None, initial_file_path=None, api_client=api)
    mapper.show()

    exit_code = app.exec_()
    sys.exit(exit_code)

except Exception as e:
    traceback.print_exc()
    QMessageBox.critical(None, "Error Crítico", f"No se pudo iniciar la aplicación:\n{e}")
    sys.exit(1)
