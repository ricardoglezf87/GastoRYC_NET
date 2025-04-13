# GARCA/settings/production.py
import os
from .base import * # Importa toda la configuración base

# SECURITY WARNING: keep the secret key used in production secret!
# ¡IMPORTANTE! No pongas la clave secreta directamente aquí.
# Léela desde una variable de entorno o un archivo secreto.
SECRET_KEY = 'django-insecure-u4ih8i#^0=x+j4+*b(7uwf-3m-!jo4e!!p+pww5sns#69g2_j-'
if not SECRET_KEY:
    # Puedes lanzar un error si no se define en el entorno
    raise ValueError("No se ha definido la variable de entorno DJANGO_SECRET_KEY")

# SECURITY WARNING: don't run with debug turned on in production!
DEBUG = True

ALLOWED_HOSTS = ['*'] 
    
# Database para Producción
DATABASES = {
    'default': {
        'ENGINE': 'django.db.backends.sqlite3',
        # Usamos la variable BASE_DIR importada de base.py
        'NAME': BASE_DIR / 'garca.sqlite3', # <-- Base de datos para producción
    }
    # Si usaras PostgreSQL o MySQL en producción, la configuración iría aquí,
    # leyendo credenciales de variables de entorno.
    # 'default': {
    #     'ENGINE': 'django.db.backends.postgresql',
    #     'NAME': os.environ.get('DB_NAME'),
    #     'USER': os.environ.get('DB_USER'),
    #     'PASSWORD': os.environ.get('DB_PASSWORD'),
    #     'HOST': os.environ.get('DB_HOST', 'localhost'),
    #     'PORT': os.environ.get('DB_PORT', '5432'),
    # }
}

LOGGING = {
    'version': 1,
    'disable_existing_loggers': False,
    'handlers': {
        'file': {
            'level': 'WARNING', # Loguea WARNING, ERROR, CRITICAL
            'class': 'logging.FileHandler',
            # Asegúrate de que el directorio exista y tenga permisos de escritura
            'filename': BASE_DIR / 'logs' / 'django_production.log',
        },
        'console': { # Para ver logs si corres con uWSGI/Gunicorn en foreground
            'level': 'INFO',
            'class': 'logging.StreamHandler',
        },
    },
    'loggers': {
        'django': {
            'handlers': ['file', 'console'],
            'level': 'INFO', # Captura INFO y superiores de Django
            'propagate': True,
        },
    },
}
# Crea el directorio de logs si no existe
LOGS_DIR = BASE_DIR / 'logs'
LOGS_DIR.mkdir(exist_ok=True)

print("--- Cargando configuración de PRODUCCIÓN ---") # Para confirmar qué se carga
