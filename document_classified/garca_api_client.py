import requests
import os
import json
import traceback

class ApiClient:
    """
    Cliente API unificado que combina la funcionalidad de 
    garca_client_api.py y bulk_matcher_api_client.py.
    """
    def __init__(self, base_url="http://127.0.0.1:8585/"):
        self.base_url = base_url.rstrip('/')
        self.api_base = f"{self.base_url}/api"
        
        # URLs de Endpoints (combinación de ambas clases)
        self.documents_url = f"{self.api_base}/documents"
        self.accounts_url = f"{self.base_url}/accounts/" # Nota: usa base_url directamente
        self.document_types_url = f"{self.documents_url}/document-types/"
        self.list_documents_url = f"{self.documents_url}/documents/"
        self.reprocess_document_url_template = f"{self.documents_url}/documents/{{id}}/reprocess/"
        self.create_document_ocr_url = f"{self.documents_url}/documents/create_with_ocr/"
        self.search_entries_url = f"{self.base_url}/entries/search/" # Nota: usa base_url directamente
        self.finalize_attachment_url_template = f"{self.documents_url}/documents/{{document_id}}/finalize_attachment/{{entry_id}}/"
        self.test_rules_url = f"{self.documents_url}/test-rules/"
        
    def _make_request(self, method, url, **kwargs):
        """
        Método centralizado para realizar peticiones HTTP a la API, 
        manejando la lógica común y los errores.
        (Basado en la versión de bulk_matcher_api_client.py por su timeout más largo y manejo de errores)
        """
        print(f"API Request: {method.upper()} {url}")
        files = kwargs.get('files')
        if files:
            print(f"API Request Files: {list(files.keys())}")
            # requests maneja Content-Type para multipart/form-data automáticamente
            kwargs.pop('json', None) # Evitar conflicto si se pasan files y json
        else:
            # Asegurar Content-Type para JSON cuando no se envían archivos
            headers = kwargs.get('headers', {})
            if 'Content-Type' not in headers and 'json' in kwargs:
                 headers['Content-Type'] = 'application/json'
            kwargs['headers'] = headers

        try:
            # Usamos timeout de 60s como en bulk_matcher, más permisivo para OCR/procesos largos
            response = requests.request(method, url, timeout=60, **kwargs) 
            response.raise_for_status() # Lanza HTTPError para respuestas 4xx/5xx

            # Manejo de respuesta exitosa sin contenido (204 No Content)
            if response.status_code == 204:
                 print("API Response: Success (204 No Content)")
                 return True # O None, según la semántica deseada

            # Manejo de respuesta con contenido
            if response.content:
                try:
                    # Intenta decodificar JSON
                    json_response = response.json()
                    print(f"API Response (JSON): {json_response}")
                    return json_response
                except requests.exceptions.JSONDecodeError:
                    # Éxito pero no es JSON (podría ser texto plano, HTML, etc.)
                    print(f"API Response: Success (Status {response.status_code}, No JSON Content)")
                    # Devolver True o response.text según lo que se espere
                    return True 
            else:
                 # Éxito pero sin contenido (ej. 200 OK con cuerpo vacío)
                 print(f"API Response: Success (Status {response.status_code}, Empty Content)")
                 return True

        except requests.exceptions.HTTPError as e:
            # Intenta obtener detalles del error desde la respuesta JSON si es posible
            error_detail = ""
            try:
                error_data = e.response.json()
                if isinstance(error_data, dict):
                    # Formatea errores de diccionarios (común en DRF)
                    error_detail = " - " + "; ".join([f"{k}: {v[0] if isinstance(v, list) else v}" for k, v in error_data.items()])
                else: 
                    # Si no es un dict, usa la representación string
                    error_detail = f" - {error_data}"
            except requests.exceptions.JSONDecodeError:
                # Si la respuesta de error no es JSON, usa el texto plano
                error_detail = f" - {e.response.text}"
            except Exception:
                 # Fallback por si algo más falla al procesar el error
                 error_detail = f" - Error al procesar detalle: {e.response.text}"

            print(f"Error HTTP: {e}{error_detail}")
            # Devuelve un diccionario de error estandarizado
            return {"error": f"Error del servidor ({e.response.status_code}): {error_detail.strip(' -')}"}
        except requests.exceptions.ConnectionError as e:
            print(f"Error de Conexión: {e}")
            return {"error": f"No se pudo conectar al servidor: {self.base_url}"}
        except requests.exceptions.Timeout as e:
            print(f"Error de Timeout: {e}")
            return {"error": "La petición al servidor tardó demasiado (timeout)." }
        except requests.exceptions.RequestException as e:
            # Captura cualquier otro error relacionado con requests
            print(f"Error Inesperado de Requests: {e}")
            return {"error": f"Error de red inesperado: {e}"}
        except Exception as e:
            # Captura genérica para errores inesperados no relacionados con requests
            print(f"Error inesperado en _make_request: {e}")
            traceback.print_exc()
            return {"error": f"Error inesperado en el cliente API: {e}"}

    # --- Métodos Comunes ---

    def get_accounts(self):
        """Obtiene la lista de cuentas contables."""
        url = self.accounts_url
        result = self._make_request('get', url)
        if isinstance(result, dict) and "error" in result: 
            print(f"get_accounts: Error recibido - {result['error']}")
            return [] # Devuelve lista vacía en caso de error
        return result if isinstance(result, list) else [] # Asegura devolver una lista

    def get_document_types(self):
        """Obtiene la lista de todos los tipos de documento/factura."""
        result = self._make_request('get', self.document_types_url)
        if isinstance(result, dict) and "error" in result: 
            print(f"get_document_types: Error recibido - {result['error']}")
            return []
        return result if isinstance(result, list) else []

    def get_document_type(self, type_id):
        """Obtiene los detalles de un tipo de documento específico."""
        url = f"{self.document_types_url}{type_id}/"
        result = self._make_request('get', url)
        # Devuelve el diccionario si es exitoso y no es un error, sino None
        return result if isinstance(result, dict) and "error" not in result else None

    def create_document_type(self, payload):
        """Crea un nuevo tipo de documento."""
        result = self._make_request('post', self.document_types_url, json=payload)
        # Devuelve el resultado (puede ser el objeto creado o un dict de error)
        return result if isinstance(result, dict) else None # O manejar otros tipos si es necesario

    def update_document_type(self, type_id, payload):
        """Actualiza un tipo de documento existente usando PATCH."""
        url = f"{self.document_types_url}{type_id}/"
        result = self._make_request('patch', url, json=payload)
        return result if isinstance(result, dict) else None

    # --- Métodos de bulk_matcher_api_client ---

    def get_documents(self):
        """Obtiene la lista de documentos procesados."""
        url = self.list_documents_url
        result = self._make_request('get', url)
        if isinstance(result, dict) and "error" in result: 
            print(f"get_documents: Error recibido - {result['error']}")
            return []
        return result if isinstance(result, list) else []

    def reprocess_document(self, document_id):
        """Solicita el reprocesamiento de un documento existente."""
        url = self.reprocess_document_url_template.format(id=document_id)
        result = self._make_request('post', url)
        return result # Devuelve la respuesta de la API (éxito/error)

    def create_document_with_ocr(self, file_path, ocr_text):
        """Crea un documento enviando un archivo y su texto OCR extraído."""
        url = self.create_document_ocr_url
        try:
            with open(file_path, 'rb') as f:
                files_payload = {'file': (os.path.basename(file_path), f)}
                # Asegúrate que el backend espera 'extracted_text' en 'data' y no en 'json'
                data_payload = {'extracted_text': ocr_text} 
                result = self._make_request('post', url, files=files_payload, data=data_payload)
                return result
        except FileNotFoundError:
             print(f"Error: Archivo no encontrado en create_document_with_ocr: {file_path}")
             return {"error": f"Archivo no encontrado: {file_path}"}
        except IOError as e:
             print(f"Error de I/O abriendo archivo en create_document_with_ocr: {file_path} - {e}")
             return {"error": f"Error al leer archivo local: {e}"}
        except Exception as e:
             # Captura otros posibles errores (permisos, etc.)
             print(f"Error procesando archivo en create_document_with_ocr: {e}")
             traceback.print_exc()
             return {"error": f"Error al procesar archivo local: {e}"}

    def get_accounting_entries(self, account_id, start_date, end_date, amount=None):
        """Busca asientos contables según criterios."""
        url = self.search_entries_url
        params = {'account_id': account_id, 'start_date': start_date, 'end_date': end_date}
        if amount is not None: 
            params['amount'] = amount
        print(f"Buscando asientos contables con params: {params}") # DEBUG
        result = self._make_request('get', url, params=params)
        if isinstance(result, dict) and "error" in result: 
            print(f"get_accounting_entries: Error recibido - {result['error']}")
            return []
        return result if isinstance(result, list) else []

    def associate_entry(self, document_id, entry_id):
        """
        Asocia un documento a un asiento contable.
        NOTA: La URL para esta operación no estaba claramente definida. 
        Se asume una URL o se necesita ajustar. Aquí usamos una placeholder.
        """
        # Reemplaza 'URL_PARA_ASOCIAR' con la URL correcta del endpoint
        url_placeholder = f"{self.api_base}/associate-entry/" # ¡AJUSTAR ESTA URL!
        payload = {'document_id': document_id, 'entry_id': entry_id}
        print(f"Asociando documento {document_id} con asiento {entry_id} en URL: {url_placeholder}")
        result = self._make_request('post', url_placeholder, json=payload) 
        return result

    def finalize_document_attachment(self, document_id, entry_id):
        """Solicita al backend adjuntar el archivo de un documento a un asiento y eliminar el documento."""
        url = self.finalize_attachment_url_template.format(document_id=document_id, entry_id=entry_id)
        # Es una petición POST sin cuerpo de datos (la información va en la URL)
        result = self._make_request('post', url) 
        return result

    # --- Métodos de garca_client_api ---

    def test_rules(self, file_path, rules):
        """Prueba reglas de extracción enviando un archivo y las reglas en formato JSON."""
        url = self.test_rules_url # Usar la URL definida en __init__
        try:
            # Convertir el diccionario/lista de reglas a una cadena JSON
            rules_json_string = json.dumps(rules)
        except TypeError as e:
            print(f"Error convirtiendo reglas a JSON: {e}")
            return {"error": f"Error interno al preparar reglas para enviar: {e}"}
        except Exception as e:
            print(f"Error inesperado convirtiendo reglas a JSON: {e}")
            return {"error": f"Error inesperado al preparar reglas: {e}"}

        try:
            # Abrir el archivo en modo binario para el envío
            with open(file_path, 'rb') as f:
                # Preparar payload multipart/form-data
                # El backend debe esperar 'file' como archivo y 'rules' como campo de datos (string JSON)
                files_payload = {'file': (os.path.basename(file_path), f)}
                data_payload = {'rules': rules_json_string} 

                # Realizar la petición POST
                result = self._make_request('post', url, files=files_payload, data=data_payload)
                return result # Devuelve la respuesta del backend (éxito o error)

        except FileNotFoundError:
             print(f"Error: Archivo no encontrado en test_rules: {file_path}")
             return {"error": f"Archivo no encontrado: {file_path}"}
        except IOError as e:
             print(f"Error de I/O abriendo archivo en test_rules: {file_path} - {e}")
             return {"error": f"Error al leer archivo local: {e}"}
        except Exception as e:
             # Captura otros posibles errores
             print(f"Error procesando archivo o enviando en test_rules: {e}")
             traceback.print_exc()
             return {"error": f"Error al procesar archivo local o enviar petición: {e}"}

