# c:\Users\rgonzafa\source\repos\GastoRYC_NET\async_tasks\tasks.py
from celery import shared_task
from django.db import OperationalError, transaction
# Asegúrate de importar Sum desde django.db.models
from django.db.models import F, Sum as DbSum # Renombrado para evitar conflicto con la función sum()
# No necesitas importar 'models' de accounts así, es mejor importar Account directamente
# from accounts import models
from accounts.models import Account
from entries.models import Entry
from django.utils import timezone
# Ya no necesitas 'db_models' si importas Sum como DbSum
# from django.db import models as db_models
import os
import datetime
import zipfile
from django.conf import settings
from django.core.exceptions import ImproperlyConfigured
import logging
from django.core.cache import cache

logger = logging.getLogger(__name__)

# --- hello_world, calculate_inicial_balances, recalculate_balances_after_date sin cambios ---
@shared_task(bind=True)
def hello_world(self):
    print("¡Hola Mundo!")
    return "¡Hola Mundo!"

@shared_task(bind=True, autoretry_for=(Exception,), retry_kwargs={'max_retries': 5, 'countdown': 1})
def calculate_inicial_balances(self):
    print ('Calcula los balances iniciales para todas las cuentas')
    # Es más eficiente empezar desde las hojas (cuentas sin hijos) hacia arriba,
    # pero recalcular todo desde una fecha muy antigua también funciona.
    # La implementación actual llama a recalculate_balances_after_date para cada cuenta,
    # y esa a su vez llama a update_account_balance_task, que ahora propagará hacia arriba.
    accounts = Account.objects.all()
    for account in accounts:
        # Esta llamada desencadenará la actualización en cascada hacia arriba
        # a través de update_account_balance_task
        recalculate_balances_after_date.delay( # Usar .delay() para que sea asíncrono
            timezone.datetime(1900, 1, 1).date(),
            account.id
        )
        # El print aquí puede ser prematuro, la tarea se está ejecutando en background
        # logger.info(f'Tarea de recálculo iniciada para cuenta {account.name}') # Mejor usar logger
    return "Tareas de recálculo de balances iniciales iniciadas"

@shared_task(bind=True, autoretry_for=(Exception,), retry_kwargs={'max_retries': 5, 'countdown': 1})
def recalculate_balances_after_date(self, date, account_id):
    """
    Recalcula los balances *dentro de las entradas* posteriores a una fecha
    para una cuenta específica y luego actualiza el balance total de la cuenta.
    """
    try:
        account = Account.objects.get(pk=account_id) # Verificar que la cuenta existe primero
    except Account.DoesNotExist:
        logger.warning(f"Intento de recalcular balances para cuenta inexistente ID: {account_id}")
        return f"Error: Cuenta {account_id} no encontrada."

    with transaction.atomic():
        # Obtener balance acumulado *antes* de la fecha de inicio directamente de las transacciones
        # Esto es más preciso que buscar la última entrada antes de la fecha.
        previous_balance_agg = account.transaction_set.filter(
            entry__date__lt=date
        ).aggregate(
            balance=DbSum(F('debit')) - DbSum(F('credit'))
        )
        current_balance = previous_balance_agg['balance'] or 0

        # Obtener entradas relevantes ordenadas
        entries = Entry.objects.filter(
            date__gte=date,
            transactions__account_id=account_id
        ).prefetch_related('transactions').order_by('date', 'id').distinct() # distinct() es importante

        for entry in entries:
            # Asegurarnos de que balance es un diccionario
            if not isinstance(entry.balance, dict):
                entry.balance = {}

            # Calcular el impacto de *esta* entrada en el balance de la cuenta
            entry_impact = entry.transactions.filter(
                account_id=account_id
            ).aggregate(
                impact=DbSum(F('debit')) - DbSum(F('credit'))
            )['impact'] or 0

            current_balance += entry_impact

            # Actualizar el balance *en el momento de esta entrada* en el campo JSON
            entry.balance[str(account_id)] = float(current_balance) # Guardar como float
            entry.save(update_fields=['balance']) # Guardar solo el campo modificado

    # Después de actualizar los balances históricos en las entradas,
    # lanzar la tarea para actualizar el balance *total* de la cuenta (Account.balance)
    # Esta tarea ahora incluirá a los hijos y propagará hacia arriba.
    update_account_balance_task.delay(account_id)

    logger.info(f"Balances históricos recalculados para cuenta {account_id} desde {date}")
    return f"Balances históricos recalculados para cuenta {account_id}"


@shared_task(bind=True, max_retries=3, default_retry_delay=5)
def update_account_balance_task(self, account_id):
    """
    Actualiza el campo 'balance' de una cuenta sumando su balance de transacciones
    directas y los balances de sus hijos directos. Propaga el cambio al padre si es necesario.
    """
    try:
        # Usar select_for_update para bloquear la fila durante la transacción
        # y evitar condiciones de carrera si varias tareas intentan actualizarla a la vez.
        account = Account.objects.select_for_update().get(pk=account_id)

        with transaction.atomic():
            # 1. Calcular balance de las transacciones DIRECTAS de esta cuenta
            own_balance_agg = account.transaction_set.aggregate(
                balance=DbSum(F('debit')) - DbSum(F('credit'))
            )
            own_balance = own_balance_agg['balance'] or 0

            # 2. Sumar los balances de los HIJOS DIRECTOS
            # (Asumimos que los balances de los hijos ya están actualizados o se actualizarán)
            children_balance_agg = account.children.aggregate(
                total_children_balance=DbSum('balance')
            )
            children_balance_sum = children_balance_agg['total_children_balance'] or 0

            # 3. Calcular el nuevo balance total
            new_total_balance = own_balance + children_balance_sum

            # 4. Actualizar si el balance ha cambiado
            balance_changed = False
            if account.balance != new_total_balance:
                account.balance = new_total_balance
                # No necesitas last_updated aquí si solo actualizas el balance
                account.save(update_fields=['balance'])
                balance_changed = True
                logger.info(f"Balance actualizado para la cuenta {account.name} ({account_id}): {new_total_balance}")

                # Invalida caché si es necesario
                cache_key = f'account_balance_{account_id}'
                cache.delete(cache_key)
                logger.debug(f"Caché invalidada para {cache_key}")

        # 5. Si el balance cambió y la cuenta tiene un padre,
        #    lanzar la tarea para actualizar el balance del padre.
        if balance_changed and account.parent_id: # Usar parent_id evita cargar el objeto padre innecesariamente
             logger.info(f"Propagando actualización de balance de {account.name} ({account_id}) a padre {account.parent_id}")
             # Lanzar la tarea para el padre de forma asíncrona
             update_account_balance_task.delay(account.parent_id)

        return f"Balance actualizado para la cuenta {account_id}"

    except Account.DoesNotExist:
        logger.error(f"Cuenta {account_id} no encontrada durante la actualización de balance.")
        return f"Error: Cuenta {account_id} no encontrada."
    except OperationalError as exc:
         # Error de bloqueo de base de datos, reintentar es apropiado
         logger.warning(f"Error de base de datos (OperationalError) actualizando balance para cuenta {account_id}: {exc}. Reintentando...")
         raise self.retry(exc=exc)
    except Exception as exc:
        logger.exception(f"Error inesperado actualizando balance para cuenta {account_id}: {exc}. Reintentando...")
        raise self.retry(exc=exc)


# --- create_backup_view y create_backup sin cambios ---
@shared_task(bind=True, autoretry_for=(Exception,), retry_kwargs={'max_retries': 5, 'countdown': 1})
def create_backup_view(request): # El parámetro request no se usa aquí, podría eliminarse
    try:
        backup_file_path = create_backup()
        logger.info(f"Copia de seguridad creada exitosamente en: {backup_file_path}")
        return f"Copia de seguridad creada exitosamente en: {backup_file_path}"
    except Exception as e:
        logger.error(f"Error al crear la copia de seguridad: {e}", exc_info=True)
        # No relanzar la excepción aquí directamente si quieres que la tarea termine como 'SUCCESS'
        # pero con un mensaje de error. Si quieres que Celery la marque como 'FAILURE', relanza la excepción.
        # raise self.retry(exc=e) # Opcional: reintentar si falla
        return f"Error al crear la copia de seguridad: {e}"


def create_backup():
    """
    Crea una copia de seguridad copiando directamente el archivo de base de datos SQLite
    y comprimiéndolo en un archivo .zip.
    """
    # --- Validación de la configuración de la base de datos ---
    db_config = settings.DATABASES.get('default')
    if not db_config or db_config.get('ENGINE') != 'django.db.backends.sqlite3':
        logger.error("Error de configuración: La base de datos 'default' no es SQLite3 o no está definida.")
        raise ImproperlyConfigured("La configuración de la base de datos 'default' no es SQLite3 o no está definida.")

    db_path = db_config.get('NAME')
    if not db_path:
        logger.error("Error de configuración: La ruta del archivo SQLite ('NAME') no está definida.")
        raise ImproperlyConfigured("La ruta del archivo de base de datos SQLite ('NAME') no está definida en settings.DATABASES['default'].")

    if not os.path.exists(db_path):
        logger.error(f"Error: El archivo de base de datos SQLite no se encontró en: {db_path}")
        raise FileNotFoundError(f"El archivo de base de datos SQLite no se encontró en: {db_path}")

    # --- Preparación de directorios y nombres de archivo ---
    backup_dir = os.path.join(settings.BASE_DIR, 'backups')
    os.makedirs(backup_dir, exist_ok=True)

    timestamp = datetime.datetime.now().strftime('%Y%m%d_%H%M%S')
    db_filename_in_zip = os.path.basename(db_path)
    zip_filename = f'backup_sqlite_{timestamp}.zip'
    zip_filepath = os.path.join(backup_dir, zip_filename)

    try:
        logger.info(f"Iniciando copia de seguridad de archivo SQLite: {db_path}")
        logger.info(f"Creando archivo ZIP: {zip_filepath}")

        with zipfile.ZipFile(zip_filepath, 'w', zipfile.ZIP_DEFLATED) as zipf:
            zipf.write(db_path, arcname=db_filename_in_zip)
            logger.info(f"Archivo '{os.path.basename(db_path)}' añadido al ZIP como '{db_filename_in_zip}'")

        logger.info(f"Copia de seguridad SQLite completada exitosamente en: {zip_filepath}")
        return zip_filepath

    except FileNotFoundError as e: # Captura específica por si acaso
        logger.error(f"Error de archivo no encontrado durante el backup: {e}", exc_info=True)
        raise # Relanzar para que Celery la marque como fallo
    except zipfile.BadZipFile as e:
        logger.error(f"Error al crear el archivo ZIP: {e}", exc_info=True)
        if os.path.exists(zip_filepath):
            try: os.remove(zip_filepath)
            except OSError as remove_err: logger.error(f"No se pudo eliminar el ZIP corrupto {zip_filepath}: {remove_err}")
        raise # Relanzar
    except Exception as e:
        logger.error(f"Error inesperado durante el backup de SQLite: {e}", exc_info=True)
        if os.path.exists(zip_filepath):
             try: os.remove(zip_filepath)
             except OSError as remove_err: logger.error(f"No se pudo eliminar el ZIP incompleto {zip_filepath}: {remove_err}")
        raise # Relanzar
