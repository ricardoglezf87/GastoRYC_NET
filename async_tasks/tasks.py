from celery import shared_task
from django.db import transaction
from django.db.models import F, Sum
from accounts.models import Account
from entries.models import Entry
from django.utils import timezone
import os
import datetime
import zipfile
from django.conf import settings
from django.core.exceptions import ImproperlyConfigured 

@shared_task(bind=True)
def hello_world(self):
    print("¡Hola Mundo!")
    return "¡Hola Mundo!"

@shared_task(bind=True, autoretry_for=(Exception,), retry_kwargs={'max_retries': 5, 'countdown': 1})
def calculate_inicial_balances(self):
    print ('Calcula los balances iniciales para todas las cuentas')
    accounts = Account.objects.all()
    for account in accounts:
        recalculate_balances_after_date(
            timezone.datetime(1900, 1, 1).date(),
            account.id
        )
        print(f'Balances calculados para cuenta {account.name}')
    return "Balances iniciales calculados"

@shared_task(bind=True, autoretry_for=(Exception,), retry_kwargs={'max_retries': 5, 'countdown': 1})
def recalculate_balances_after_date(self,date, account_id):
    """
    Recalcula los balances de todas las entradas posteriores a una fecha
    para una cuenta específica
    """
    with transaction.atomic():
        entries = Entry.objects.filter(
            date__gte=date,
            transactions__account_id=account_id
        ).order_by('date', 'id')

        # Obtener balance anterior
        previous_balance = Entry.objects.filter(
            date__lt=date,
            transactions__account_id=account_id
        ).aggregate(
            balance=Sum(F('transactions__debit') - F('transactions__credit'))
        )['balance'] or 0

        for entry in entries:
            # Asegurarnos de que balance es un diccionario
            if not isinstance(entry.balance, dict):
                entry.balance = {}
                
            # Actualizar balance para esta cuenta
            entry_balance = entry.transactions.filter(
                account_id=account_id
            ).aggregate(
                balance=Sum(F('debit') - F('credit'))
            )['balance'] or 0
            
            previous_balance += entry_balance
            
            # Actualizar el balance en el campo JSON
            entry.balance[str(account_id)] = float(previous_balance)
            entry.save()
    return "Balances recalculados"


@shared_task(bind=True, autoretry_for=(Exception,), retry_kwargs={'max_retries': 5, 'countdown': 1}) 
def create_backup_view(request):
    try:
        backup_file_path = create_backup()
        return f"Copia de seguridad creada exitosamente en: {backup_file_path}"
    except Exception as e:
        return f"Error al crear la copia de seguridad: {e}"


def create_backup():
    """
    Crea una copia de seguridad copiando directamente el archivo de base de datos SQLite
    y comprimiéndolo en un archivo .zip.
    """
    # --- Validación de la configuración de la base de datos ---
    db_config = settings.DATABASES.get('default')
    if not db_config or db_config.get('ENGINE') != 'django.db.backends.sqlite3':
        raise ImproperlyConfigured("La configuración de la base de datos 'default' no es SQLite3 o no está definida.")

    db_path = db_config.get('NAME')
    if not db_path:
        raise ImproperlyConfigured("La ruta del archivo de base de datos SQLite ('NAME') no está definida en settings.DATABASES['default'].")

    if not os.path.exists(db_path):
        raise FileNotFoundError(f"El archivo de base de datos SQLite no se encontró en: {db_path}")

    # --- Preparación de directorios y nombres de archivo ---
    backup_dir = os.path.join(settings.BASE_DIR, 'backups')
    os.makedirs(backup_dir, exist_ok=True)

    timestamp = datetime.datetime.now().strftime('%Y%m%d_%H%M%S')
    # Nombre del archivo SQLite dentro del ZIP
    db_filename_in_zip = os.path.basename(db_path)
    # Nombre del archivo ZIP final
    zip_filename = f'backup_sqlite_{timestamp}.zip'
    zip_filepath = os.path.join(backup_dir, zip_filename)

    try:
        print(f"Iniciando copia de seguridad de archivo SQLite: {db_path}")
        print(f"Creando archivo ZIP: {zip_filepath}")

        # --- Crear el archivo ZIP y añadir el archivo de base de datos ---
        # Copiar directamente el archivo de la base de datos al ZIP
        # Usar shutil.copy2 podría ser una opción si necesitas copiarlo primero,
        # pero añadirlo directamente al zip es más eficiente.
        # ¡Importante! Esto copia el estado actual del archivo. Si hay transacciones
        # en curso, la copia podría no ser perfectamente consistente. Para mayor
        # seguridad, se debería bloquear la base de datos o detener la aplicación,
        # pero eso no es práctico desde aquí.
        with zipfile.ZipFile(zip_filepath, 'w', zipfile.ZIP_DEFLATED) as zipf:
            zipf.write(db_path, arcname=db_filename_in_zip)
            print(f"Archivo '{os.path.basename(db_path)}' añadido al ZIP como '{db_filename_in_zip}'")

        print(f"Copia de seguridad SQLite completada exitosamente en: {zip_filepath}")
        return zip_filepath

    except FileNotFoundError as e: # Captura específica por si acaso
        print(f"Error: {e}")
        raise Exception(f"Error de archivo no encontrado durante el backup: {e}")
    except zipfile.BadZipFile as e:
        print(f"Error al crear el archivo ZIP: {e}")
        # Intentar limpiar el ZIP si se creó parcialmente
        if os.path.exists(zip_filepath):
            try: os.remove(zip_filepath)
            except OSError: pass
        raise Exception(f"Error al crear el archivo ZIP: {e}")
    except Exception as e:
        print(f"Error inesperado durante el backup de SQLite: {e}")
        # Intentar limpiar el ZIP si se creó
        if os.path.exists(zip_filepath):
            try: os.remove(zip_filepath)
            except OSError: pass
        raise Exception(f"Error inesperado durante el backup: {str(e)}")