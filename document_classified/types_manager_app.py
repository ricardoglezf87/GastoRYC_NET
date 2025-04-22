# types_manager_app.py 
import django
import sys
import os
import traceback # Para imprimir errores detallados
from PyQt5.QtWidgets import (QApplication, QWidget, QVBoxLayout, QLabel,
                             QLineEdit, QPushButton, QComboBox, QMessageBox,
                             QTextEdit,QFileDialog)
 
from PyQt5.QtCore import Qt,pyqtSlot, QStringListModel
from PyQt5.QtWidgets import QCompleter

from .garca_api_client import ApiClient

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'GARCA.settings')
try:
    django.setup()
    print("DJANGO SETUP: Django inicializado correctamente.") # Mensaje de confirmación
except Exception as e:
    print(f"DJANGO SETUP ERROR: No se pudo inicializar Django: {e}")

from .processing_logic import extract_document_data, perform_ocr

# --- Clase ApiClient (Añadido timeout y prints) ---


class MappingWindow(QWidget):
    def __init__(self, document_id, initial_file_path, api_client, parent=None): # Cambiado nombre de parámetro
        super().__init__(parent)
        print("MappingWindow __init__ started") # DEBUG
        self.document_id = document_id # Aún puede ser útil si guardas tipos asociados a docs
        self.file_path = initial_file_path # Guardar la ruta inicial (puede ser None)
        self.api_client = api_client
        self.current_document_type_id = None
        self.document_types_data = {}

        self.setWindowTitle("Gestor de Tipos de Factura - Editor de Texto")
        self.layout = QVBoxLayout(self)
        self.setMinimumSize(700, 800)
        print("MappingWindow basic setup done") # DEBUG

        # --- NUEVO: Sección de Selección de Archivo ---
        self.file_select_button = QPushButton("Seleccionar Archivo PDF")
        self.file_select_button.clicked.connect(self.select_file)
        self.layout.addWidget(self.file_select_button)

        self.selected_file_label = QLabel("Archivo seleccionado: Ninguno") # Label para mostrar ruta
        self.layout.addWidget(self.selected_file_label)
        # --------------------------------------------

        # --- Selección de Tipo (sin cambios) ---
        self.layout.addWidget(QLabel("Tipo de Factura:"))
        self.type_combo = QComboBox()
        self.type_combo.addItem("-- Crear Nuevo Tipo --", userData=None)
        self.layout.addWidget(self.type_combo)
        self.type_combo.currentIndexChanged.connect(self.on_type_selected)
        print("MappingWindow type combo setup done") # DEBUG

        # --- Visor de Texto OCR (sin cambios) ---
        self.layout.addWidget(QLabel("Texto Extraído (OCR):"))
        self.text_edit = QTextEdit()
        self.text_edit.setReadOnly(True)
        self.text_edit.setLineWrapMode(QTextEdit.NoWrap)
        self.text_edit.setFontFamily("Courier New")
        self.layout.addWidget(self.text_edit, 1)
        print("MappingWindow text viewer setup done") # DEBUG

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
        print("MappingWindow rules fields setup done") # DEBUG

        # --- Selección de Cuenta (sin cambios) ---
        self.layout.addWidget(QLabel("Cuenta Contable (Filtrable):")) # Etiqueta actualizada
        self.account_combo = QComboBox()
        
        self.account_combo.setEditable(True)
        self.account_combo.setInsertPolicy(QComboBox.NoInsert) # No añadir texto escrito como item nuevo
        self.account_combo.setPlaceholderText("Escribe para filtrar...") # Texto de ayuda

        self.account_completer = QCompleter(self) # Crear el completer
        self.account_completer.setFilterMode(Qt.MatchContains) # Filtrar si contiene el texto
        self.account_completer.setCompletionMode(QCompleter.PopupCompletion) # Mostrar sugerencias en popup
        self.account_completer.setCaseSensitivity(Qt.CaseInsensitive) # Ignorar mayúsculas/minúsculas

        self.account_combo.setCompleter(self.account_completer)

        self.layout.addWidget(self.account_combo)
        print("MappingWindow account combo setup done") # DEBUG

        # --- Botones (sin cambios) ---
        self.save_button = QPushButton("Guardar Tipo")
        self.save_button.clicked.connect(self.save_type)
        self.layout.addWidget(self.save_button)
        self.test_button = QPushButton("Probar Reglas con Texto Actual")
        self.test_button.clicked.connect(self.test_template_rules)
        self.layout.addWidget(self.test_button)
        print("MappingWindow buttons setup done") # DEBUG

        # --- Carga inicial ---
        # Ya no cargamos el texto aquí, se hará al seleccionar archivo
        # print("Calling load_document_text...")
        # self.load_document_text()
        # print("Finished load_document_text call.")

        # Actualizar label si se pasó una ruta inicial
        if self.file_path:
             self.selected_file_label.setText(f"Archivo seleccionado: {os.path.basename(self.file_path)}")
             # Podrías llamar a load_document_text aquí si quieres cargar el inicial
             # self.load_document_text()

        print("Calling load_document_types...")
        self.load_document_types()
        print("Finished load_document_types call.")

        print("Calling load_accounts...")
        self.load_accounts()
        print("Finished load_accounts call.")
        print("MappingWindow __init__ finished") # DEBUG
    # --- FIN MODIFICADO __init__ ---

    # --- NUEVO MÉTODO select_file ---
    def select_file(self):
        """Abre un diálogo para seleccionar un archivo PDF."""
        # Abre el diálogo, empezando en el directorio actual o el último usado
        # Filtra para mostrar solo archivos PDF
        options = QFileDialog.Options()
        # options |= QFileDialog.DontUseNativeDialog # Descomenta si el diálogo nativo da problemas
        file_path, _ = QFileDialog.getOpenFileName(
            self,
            "Seleccionar Factura PDF",
            "", # Directorio inicial (vacío usa el último o el actual)
            "Archivos PDF (*.pdf);;Todos los archivos (*)",
            options=options
        )

        if file_path: # Si el usuario seleccionó un archivo
            print(f"Archivo seleccionado: {file_path}")
            self.file_path = file_path
            # Actualiza el label para mostrar el nombre del archivo
            self.selected_file_label.setText(f"Archivo seleccionado: {os.path.basename(self.file_path)}")
            # Llama a la función para cargar y hacer OCR del nuevo archivo
            self.load_document_text()
        else:
            print("Selección de archivo cancelada.")

    # --- MÉTODO RENOMBRADO Y MODIFICADO ---
    def load_document_text(self):
        """Realiza OCR en el PDF (self.file_path) y muestra el texto."""
        if not self.file_path or not os.path.exists(self.file_path):
             error_msg = "Por favor, selecciona un archivo PDF válido primero."
             print(f"load_document_text: {error_msg}")
             self.text_edit.setPlainText(error_msg)
             # Podrías mostrar un QMessageBox también
             # QMessageBox.warning(self, "Archivo no válido", error_msg)
             return

        print(f"load_document_text: Intentando cargar y hacer OCR en: {self.file_path}") # DEBUG
        self.text_edit.setPlainText("Realizando OCR, por favor espera...") # Mensaje temporal
        QApplication.processEvents() # Forzar actualización de la UI

        try:
            extracted_text = perform_ocr(self.file_path)
            if extracted_text is not None:
                self.text_edit.setPlainText(extracted_text)
                print("load_document_text: Texto OCR cargado en la interfaz.") # DEBUG
            else:
                error_msg = "Error durante el OCR. Revisa la consola para más detalles."
                print(f"load_document_text: {error_msg}") # DEBUG
                self.text_edit.setPlainText(error_msg)
        except Exception as e:
             error_msg = f"Error inesperado al cargar texto OCR: {e.__class__.__name__}: {e}"
             print(f"load_document_text: {error_msg}") # DEBUG
             traceback.print_exc()
             self.text_edit.setPlainText(error_msg)
             QMessageBox.critical(self, "Error Crítico OCR", error_msg)

    def load_document_types(self):
        """Carga los tipos de factura desde la API y los añade al ComboBox."""
        print("load_document_types: Cargando tipos...")
        self.type_combo.blockSignals(True) # Bloquear señales mientras se modifica
        # Limpiar excepto el item "-- Crear Nuevo Tipo --"
        while self.type_combo.count() > 1:
            self.type_combo.removeItem(1)
        self.document_types_data = {} # Limpiar datos cacheados

        types = self.api_client.get_document_types()
        if types: # Si la API devolvió una lista (puede estar vacía)
            print(f"load_document_types: {len(types)} tipos recibidos.")
            # Ordenar por nombre para facilitar la búsqueda
            try:
                # Usar lower() para ordenación insensible a mayúsculas/minúsculas
                types_sorted = sorted(types, key=lambda x: str(x.get('name', '')).lower())
            except TypeError: # Por si algún nombre no es string o comparable
                print("load_document_types: Advertencia - No se pudo ordenar los tipos por nombre.")
                types_sorted = types # Usar sin ordenar si falla

            for inv_type in types_sorted:
                type_id = inv_type.get('id')
                type_name = inv_type.get('name')
                if type_id is not None and type_name:
                    self.type_combo.addItem(type_name, userData=type_id)
                    # Guardar los datos completos del tipo para uso posterior
                    self.document_types_data[type_id] = inv_type
            print(f"load_document_types: {len(self.document_types_data)} tipos añadidos al ComboBox.")
        else: # Si la API devolvió [] por error o no encontró tipos
            print("load_document_types: No se encontraron tipos o hubo un error al cargarlos.")
            # Podrías mostrar un mensaje en la UI o deshabilitar el combo
            # self.type_combo.addItem("Error al cargar tipos")
            # self.type_combo.setEnabled(False)
        self.type_combo.blockSignals(False) # Desbloquear señales

    @pyqtSlot(int) # Decorador para indicar que es un slot de Qt
    def on_type_selected(self, index):
        """Se ejecuta cuando el usuario selecciona un item en el ComboBox de tipos."""
        selected_id = self.type_combo.itemData(index)
        print(f"on_type_selected: Índice {index}, ID seleccionado: {selected_id}")

        if selected_id is None: # Opción "-- Crear Nuevo Tipo --" seleccionada
            self.current_document_type_id = None
            self.clear_type_fields()
            print("on_type_selected: Preparado para crear nuevo tipo.")
        else:
            self.current_document_type_id = selected_id
            # Cargar datos del tipo seleccionado
            # Intentar usar los datos cacheados primero
            type_data = self.document_types_data.get(selected_id)
            # Verificar si los datos cacheados tienen las reglas (la lista inicial podría no incluirlas)
            if type_data and 'extraction_rules' in type_data:
                 print(f"on_type_selected: Cargando datos desde caché para tipo ID: {selected_id}")
                 self.populate_type_fields(type_data)
            else:
                 # Si no estaba en caché o faltaban reglas, buscarlo en la API
                 print(f"on_type_selected: Obteniendo detalles de API para tipo ID: {selected_id}")
                 type_details = self.api_client.get_document_type(selected_id)
                 if type_details: # Si get_document_type devolvió un diccionario (sin error)
                     self.document_types_data[selected_id] = type_details # Actualizar caché con detalles completos
                     self.populate_type_fields(type_details)
                 else: # Si get_document_type devolvió None (error)
                     error_detail = f"No se pudo obtener detalles para el tipo ID {selected_id}."
                     print(f"Error cargando detalles: {error_detail}")
                     QMessageBox.warning(self, "Error", error_detail)
                     self.clear_type_fields() # Limpiar si falla la carga
                     # Podrías volver a seleccionar "-- Crear Nuevo Tipo --"
                     # self.type_combo.setCurrentIndex(0)

    def clear_type_fields(self):
        """Limpia los campos de entrada de reglas."""
        self.name_input.clear()
        self.identifier_keyword_input.clear()
        self.date_regex_input.clear()
        self.total_regex_input.clear()
        # Limpiar otros campos de reglas que añadas
        # Limpiar también la selección de cuenta si la usas asociada al tipo
        self.account_combo.setCurrentIndex(1) # Asumiendo que el índice 0 es "-- Sin asignar --"
        print("clear_type_fields: Campos limpiados.")

    def populate_type_fields(self, type_data):
        """Rellena los campos de entrada con los datos de un documentType."""
        self.clear_type_fields() # Limpiar primero
        self.name_input.setText(type_data.get('name', ''))
        rules = type_data.get('extraction_rules', {})
        if not isinstance(rules, dict): rules = {} # Asegurar que sea un dict

        # Rellenar campos basados en las reglas
        identifier_rule = rules.get('identifier', {})
        if identifier_rule.get('type') == 'keyword':
            self.identifier_keyword_input.setText(identifier_rule.get('value', ''))
        # Añadir lógica para otros tipos de identificador si los tienes (ej. regex)
        # elif identifier_rule.get('type') == 'regex':
        #     self.identifier_regex_input.setText(identifier_rule.get('pattern', ''))

        date_rule = rules.get('date', {})
        if date_rule.get('type') == 'regex':
            self.date_regex_input.setText(date_rule.get('pattern', ''))
        # Añadir lógica para otros tipos de regla de fecha (ej. keyword_proximity)

        total_rule = rules.get('total', {})
        if total_rule.get('type') == 'regex':
            self.total_regex_input.setText(total_rule.get('pattern', ''))
        # Añadir lógica para otros tipos de regla de total

        # Rellenar otros campos que tengas...

        # Seleccionar la cuenta asociada si existe en el tipo y en el combo
        account_id = type_data.get('account') # Si el tipo tiene un campo 'account'
        if account_id is not None:
            index = self.account_combo.findData(account_id)
            if index >= 0:
                self.account_combo.setCurrentIndex(index)
            else:
                print(f"Advertencia: Cuenta ID {account_id} del tipo no encontrada en el ComboBox.")
                self.account_combo.setCurrentIndex(0) # Volver a "-- Sin asignar --"
        else:
            self.account_combo.setCurrentIndex(0) # Seleccionar "-- Sin asignar --"

        print(f"populate_type_fields: Campos rellenados para tipo: {type_data.get('name')}")

    def load_accounts(self):
        """Carga las cuentas contables desde la API y las añade al ComboBox."""
        print("load_accounts: Cargando cuentas contables...") # DEBUG
        self.account_combo.clear() # Limpiar combo
        self.account_combo.setEnabled(False) # Deshabilitar mientras carga
        account_display_names = [] # Lista para guardar los textos a mostrar

        try:
            accounts = self.api_client.get_accounts()
            if accounts:
                print(f"load_accounts: Cuentas recibidas: {len(accounts)}") # DEBUG
                try:
                    accounts_sorted = sorted(accounts, key=lambda x: str(x.get('get_full_hierarchy', x.get('name', ''))))
                except TypeError:
                    print("load_accounts: Advertencia: No se pudo ordenar las cuentas.") # DEBUG
                    accounts_sorted = accounts

                added_count = 0
                self.account_combo.addItem("-- Sin asignar --", userData=None) # Opción por defecto
                account_display_names.append("-- Sin asignar --") # Añadir a la lista del completer

                for acc in accounts_sorted:
                    acc_id = acc.get('id')
                    display_name = acc.get('get_full_hierarchy', acc.get('name'))
                    if acc_id is not None and display_name:
                        self.account_combo.addItem(display_name, userData=acc_id)
                        account_display_names.append(display_name) # <-- Guardar texto para el completer
                        added_count += 1
                    else:
                        print(f"load_accounts: Advertencia: Cuenta omitida por ID o nombre nulo: {acc}") # DEBUG
                print(f"load_accounts: {added_count} cuentas cargadas en el ComboBox.") # DEBUG

                if added_count > 0:
                    self.account_combo.setEnabled(True) # Habilitar si hay cuentas
                else:
                    self.account_combo.clear()
                    self.account_combo.addItem("No hay cuentas disponibles", userData=None)
                    account_display_names = ["No hay cuentas disponibles"] # Actualizar lista

            else:
                print("load_accounts: Error al cargar cuentas (revisar error API previo).") # DEBUG
                self.account_combo.addItem("Error al cargar cuentas", userData=None)
                account_display_names = ["Error al cargar cuentas"] # Actualizar lista

            # --- Configurar el modelo para el Completer ---
            self.account_model = QStringListModel(account_display_names, self)
            self.account_completer.setModel(self.account_model)
            # ---------------------------------------------

        except Exception as e:
            # ... (manejo de excepción sin cambios) ...
            self.account_combo.clear()
            self.account_combo.addItem("Error crítico al cargar cuentas", userData=None)

    def _build_rules_payload(self):
        """Construye el diccionario de reglas a partir de los campos de entrada."""
        rules = {}
        # Obtener valores de los QLineEdit
        identifier_val = self.identifier_keyword_input.text().strip()
        date_val = self.date_regex_input.text().strip()
        total_val = self.total_regex_input.text().strip()

        # Construir el diccionario de reglas (ajusta según tu estructura JSON)
        # Solo añadir la regla si el campo tiene valor
        if identifier_val: rules["identifier"] = {"type": "keyword", "value": identifier_val}
        if date_val: rules["date"] = {"type": "regex", "pattern": date_val}
        if total_val: rules["total"] = {"type": "regex", "pattern": total_val}
        # Añade aquí la lógica para otros campos de reglas que tengas

        print(f"_build_rules_payload: Reglas construidas: {rules}") # DEBUG
        return rules

    def save_type(self):
        """Guarda el tipo actual (crea uno nuevo o actualiza uno existente)."""
        print("save_type: Intentando guardar tipo...")
        rules = self._build_rules_payload()
        name = self.name_input.text().strip()
        # Obtener cuenta seleccionada (si la asocias al tipo)
        account_id = self.account_combo.currentData() # Devuelve None si es "-- Sin asignar --"

        if not name:
            QMessageBox.warning(self, "Faltan Datos", "Introduce un nombre para el tipo.")
            return
        # Considera si quieres permitir guardar sin reglas
        # if not rules:
        #      reply = QMessageBox.question(self, "Sin Reglas",
        #                                   "¿Guardar tipo sin reglas de extracción definidas?",
        #                                   QMessageBox.Yes | QMessageBox.No, QMessageBox.No)
        #      if reply == QMessageBox.No:
        #          return

        payload = {
            "name": name,
            "extraction_rules": rules,
            "account": account_id # Añadir si el modelo documentType tiene relación con Account
        }
        print(f"save_type: Payload construido: {payload}")

        result = None
        if self.current_document_type_id is None:
            # --- Crear Nuevo Tipo ---
            print(f"save_type: Llamando a create_document_type...")
            result = self.api_client.create_document_type(payload)
            print(f"save_type: Respuesta de create_document_type: {result}")

            if isinstance(result, dict) and 'id' in result and "error" not in result:
                 new_id = result['id']
                 QMessageBox.information(self, "Éxito", f"Tipo '{name}' (ID: {new_id}) creado correctamente.")
                 # Actualizar la lista de tipos en el ComboBox y seleccionar el nuevo
                 self.load_document_types()
                 index = self.type_combo.findData(new_id)
                 if index >= 0:
                     self.type_combo.setCurrentIndex(index)
            else:
                 # Mostrar error detallado
                 error_msg = self._format_api_error(result, "crear")
                 QMessageBox.critical(self, "Error API", error_msg)

        else:
            # --- Actualizar Tipo Existente ---
            print(f"save_type: Llamando a update_document_type para ID: {self.current_document_type_id}...")
            result = self.api_client.update_document_type(self.current_document_type_id, payload)
            print(f"save_type: Respuesta de update_document_type: {result}")

            if isinstance(result, dict) and 'id' in result and "error" not in result:
                 updated_id = result['id']
                 QMessageBox.information(self, "Éxito", f"Tipo '{name}' (ID: {updated_id}) actualizado correctamente.")
                 # Actualizar datos en caché y nombre en ComboBox si cambió
                 self.document_types_data[updated_id] = result # Actualizar caché
                 current_index = self.type_combo.currentIndex()
                 # Comprobar si el ID sigue siendo el mismo antes de actualizar texto
                 if self.type_combo.itemData(current_index) == updated_id and self.type_combo.itemText(current_index) != name:
                      self.type_combo.setItemText(current_index, name) # Actualizar texto
            else:
                 # Mostrar error detallado
                 error_msg = self._format_api_error(result, "actualizar")
                 QMessageBox.critical(self, "Error API", error_msg)

    def _format_api_error(self, result, action="guardar"):
        """Formatea un mensaje de error a partir de la respuesta de la API."""
        error_msg = f"Error desconocido al {action} el tipo."
        if isinstance(result, dict):
            if "error" in result: # Error genérico del _make_request
                error_msg = result['error']
            else: # Intentar parsear errores de validación de DRF
                try:
                    # Une los errores de cada campo
                    error_details = "\n".join([f"- {k}: {v[0] if isinstance(v, list) else v}" for k, v in result.items()])
                    if error_details:
                         error_msg = f"No se pudo {action} el tipo debido a errores:\n{error_details}"
                    # Manejar error 'non_field_errors' si existe
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
        print("test_template_rules: Iniciando prueba local con texto actual...") # DEBUG
        rules = self._build_rules_payload() # Obtener reglas de los campos
        if not rules:
             QMessageBox.warning(self, "Sin Reglas", "Define al menos una regla para probar.")
             return

        # Obtener el texto directamente del QTextEdit
        current_text = self.text_edit.toPlainText()

        if not current_text or "Realizando OCR" in current_text or "Error" in current_text:
             QMessageBox.warning(self, "Sin Texto", "No hay texto OCR válido cargado para probar.")
             return

        print("test_template_rules: Extrayendo datos localmente del texto actual...") # DEBUG

        # Extraer datos localmente usando las reglas y el texto actual
        extracted_data = extract_document_data(current_text, rules)
        print(f"test_template_rules: Datos extraídos localmente: {extracted_data}") # DEBUG

        # Mostrar el resultado (lógica sin cambios)
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


# --- Bloque Principal (con manejo de errores y prints) ---
if __name__ == '__main__':
   app = QApplication.instance()
if app is None:
    print("__main__: Creando nueva QApplication.") # DEBUG
    app = QApplication(sys.argv)
else:
    print("__main__: Usando QApplication existente.") # DEBUG

# --- Ya no se define el archivo aquí ---
# test_file_path = r"G:\Mi unidad\Pruebas\Factura Vizenter-1.pdf"
# print(f"__main__: Verificando archivo de prueba: {test_file_path}")
# if not os.path.exists(test_file_path):
#      print(f"__main__: Error: Archivo de prueba no existe: {test_file_path}")
#      QMessageBox.critical(None, "Error Archivo", f"Archivo de prueba no existe:\n{test_file_path}")
#      sys.exit(1)
# else:
#      print("__main__: Archivo de prueba encontrado.")
# -------------------------------------------

print("__main__: Creando ApiClient...") # DEBUG
api = ApiClient()

try:
    print("__main__: Creando MappingWindow...") # DEBUG
    # Pasar None como ruta inicial del archivo
    mapper = MappingWindow(document_id=None, initial_file_path=None, api_client=api)
    print("__main__: MappingWindow creado.") # DEBUG
    print("__main__: Llamando a mapper.show()...") # DEBUG
    mapper.show()
    print("__main__: mapper.show() llamado.") # DEBUG

    print("__main__: Iniciando bucle de eventos app.exec_()...") # DEBUG
    exit_code = app.exec_()
    print(f"__main__: app.exec_() finalizado con código: {exit_code}") # DEBUG
    sys.exit(exit_code)

except Exception as e:
    print(f"__main__: Error crítico al inicializar/mostrar ventana: {e}") # DEBUG
    traceback.print_exc()
    QMessageBox.critical(None, "Error Crítico", f"No se pudo iniciar la aplicación:\n{e}")
    sys.exit(1)
