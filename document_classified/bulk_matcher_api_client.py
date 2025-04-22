import requests
import os
import traceback


class ApiClient:
    def __init__(self, base_url="http://127.0.0.1:8000/"):
        self.base_url = base_url.rstrip('/')
        self.api_base = f"{self.base_url}/api"
        self.documents_url = f"{self.api_base}/documents"
        self.accounts_url = f"{self.api_base}/accounts/"
        self.document_types_url = f"{self.documents_url}/document-types/"
        self.list_documents_url = f"{self.documents_url}/documents/"
        self.reprocess_document_url_template = f"{self.documents_url}/documents/{{id}}/reprocess/"
        self.create_document_ocr_url = f"{self.documents_url}/documents/create_with_ocr/"
        self.search_entries_url = f"{self.documents_url}/entries/search/"
        self.finalize_attachment_url_template = f"{self.documents_url}/documents/{{document_id}}/finalize_attachment/{{entry_id}}/"

    def _make_request(self, method, url, **kwargs):
        # ... (código sin cambios) ...
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
        url = self.list_documents_url
        result = self._make_request('get', url)
        if isinstance(result, dict) and "error" in result: return []
        return result if isinstance(result, list) else []

    def reprocess_document(self, document_id):
        url = self.reprocess_document_url_template.format(id=document_id)
        result = self._make_request('post', url)
        return result

    def get_accounts(self):
        url = self.accounts_url
        result = self._make_request('get', url)
        if isinstance(result, dict) and "error" in result: return []
        return result if isinstance(result, list) else []

    def get_document_types(self):
        result = self._make_request('get', self.document_types_url)
        if isinstance(result, dict) and "error" in result: return []
        return result if isinstance(result, list) else []

    def get_document_type(self, type_id):
        url = f"{self.document_types_url}{type_id}/"
        result = self._make_request('get', url)
        return result if isinstance(result, dict) and "error" not in result else None

    def create_document_type(self, payload):
        result = self._make_request('post', self.document_types_url, json=payload)
        return result if isinstance(result, dict) else None

    def update_document_type(self, type_id, payload):
        url = f"{self.document_types_url}{type_id}/"
        result = self._make_request('patch', url, json=payload)
        return result if isinstance(result, dict) else None

    def create_document_with_ocr(self, file_path, ocr_text):
        url = self.create_document_ocr_url
        try:
            with open(file_path, 'rb') as f:
                files_payload = {'file': (os.path.basename(file_path), f)}
                data_payload = {'extracted_text': ocr_text}
                result = self._make_request('post', url, files=files_payload, data=data_payload)
                return result
        except FileNotFoundError:
             print(f"Error: Archivo no encontrado en create_document_with_ocr: {file_path}")
             return {"error": f"Archivo no encontrado: {file_path}"}
        except Exception as e:
             print(f"Error abriendo o enviando archivo en create_document_with_ocr: {e}")
             traceback.print_exc()
             return {"error": f"Error al procesar archivo local: {e}"}

    def get_accounting_entries(self, account_id, start_date, end_date, amount=None):
        url = self.search_entries_url
        params = {'account_id': account_id, 'start_date': start_date, 'end_date': end_date}
        if amount is not None: params['amount'] = amount
        print(f"Buscando asientos contables: {params}") # DEBUG
        result = self._make_request('get', url, params=params)
        if isinstance(result, dict) and "error" in result: return []
        return result if isinstance(result, list) else []

    def associate_entry(self, document_id, entry_id):
        url = self.associate_entry_url
        payload = {'document_id': document_id, 'entry_id': entry_id}
        result = self._make_request('post', url, json=payload)
        return result
    
    def finalize_document_attachment(self, document_id, entry_id):
        """Solicita al backend adjuntar archivo a entry y eliminar document doc."""
        url = self.finalize_attachment_url_template.format(document_id=document_id, entry_id=entry_id)
        result = self._make_request('post', url) # Petición POST sin cuerpo
        return result