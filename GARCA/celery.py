import os
from celery import Celery
from django.conf import settings

# Establecer variables de entorno
os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'GARCA.settings')

# Crear instancia de Celery
app = Celery('GARCA')

# Configuración usando objeto settings de Django
app.config_from_object('django.conf:settings', namespace='CELERY')

# Autodescubrir tareas
app.autodiscover_tasks(lambda: settings.INSTALLED_APPS)

@app.task(bind=True)
def debug_task(self):
    print(f'Request: {self.request!r}')