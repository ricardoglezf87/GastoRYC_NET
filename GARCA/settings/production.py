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
DEBUG = False # ¡Fundamental para producción!

# Define los hosts/dominios permitidos para tu sitio en producción
# Ejemplo: ALLOWED_HOSTS = ['www.tusitio.com', 'tusitio.com']
# Lee desde una variable de entorno si es posible, separada por comas
allowed_hosts_str = os.environ.get('DJANGO_ALLOWED_HOSTS')
if allowed_hosts_str:
    ALLOWED_HOSTS = allowed_hosts_str.split(',')
else:
    # Define un valor por defecto o lanza un error si prefieres
    ALLOWED_HOSTS = ['*'] # O ['tu-dominio.com'] si siempre es el mismo
    print("ADVERTENCIA: DJANGO_ALLOWED_HOSTS no está configurado. La aplicación podría no funcionar.")


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

# --- Configuraciones de Seguridad Adicionales para Producción ---

# Asegúrate de que las cookies CSRF y de sesión solo se envíen por HTTPS
CSRF_COOKIE_SECURE = True
SESSION_COOKIE_SECURE = True

# HSTS (Strict Transport Security) - Descomenta si tu sitio SIEMPRE usa HTTPS
# Ayuda a prevenir ataques man-in-the-middle
# SECURE_HSTS_SECONDS = 31536000 # 1 año
# SECURE_HSTS_INCLUDE_SUBDOMAINS = True
# SECURE_HSTS_PRELOAD = True

# Redirección a HTTPS - Descomenta si tu proxy (Nginx/Apache) no lo maneja
# SECURE_SSL_REDIRECT = True

# Referrer Policy
SECURE_REFERRER_POLICY = 'same-origin' # O 'strict-origin-when-cross-origin'

# --- Logging ---
# Configura el logging para enviar errores a un archivo o servicio externo en producción
# (Esta es una configuración básica, puedes hacerla más compleja)
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
