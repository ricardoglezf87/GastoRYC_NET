"""
Configuración base de Django para GARCA project.
"""

import os
from pathlib import Path
# Ajusta el .parent para que apunte correctamente a GastoRYC_NET/
BASE_DIR = Path(__file__).resolve().parent.parent

# Application definition
INSTALLED_APPS = [
    'accounts',
    'entries',
    'transactions',
    'attachments',
    'bank_imports',
    'reports',
    'document_classified',
    'rest_framework',
    'django.contrib.admin',
    'django.contrib.auth',
    'django.contrib.contenttypes',
    'django.contrib.sessions',
    'django.contrib.messages',
    'django.contrib.staticfiles',
    'mptt',
    'async_tasks',
]

MIDDLEWARE = [
    'django.middleware.security.SecurityMiddleware',
    'django.contrib.sessions.middleware.SessionMiddleware',
    'django.middleware.common.CommonMiddleware',
    'django.middleware.csrf.CsrfViewMiddleware',
    'django.contrib.auth.middleware.AuthenticationMiddleware',
    'django.contrib.messages.middleware.MessageMiddleware',
    'django.middleware.clickjacking.XFrameOptionsMiddleware',
]

ROOT_URLCONF = 'GARCA.urls'

TEMPLATES = [
    {
        'BACKEND': 'django.template.backends.django.DjangoTemplates',
        'DIRS': [BASE_DIR / 'GARCA' / 'templates'],
        'APP_DIRS': True,
        'OPTIONS': {
            'context_processors': [
                'django.template.context_processors.debug',
                'django.template.context_processors.request',
                'django.contrib.auth.context_processors.auth',
                'django.contrib.messages.context_processors.messages',
                'GARCA.context_processors.breadcrumbs',
            ],
        },
    },
]

WSGI_APPLICATION = 'GARCA.wsgi.application'
ASGI_APPLICATION = 'GARCA.asgi.application'
# Password validation
AUTH_PASSWORD_VALIDATORS = [
    { 'NAME': 'django.contrib.auth.password_validation.UserAttributeSimilarityValidator', },
    { 'NAME': 'django.contrib.auth.password_validation.MinimumLengthValidator', },
    { 'NAME': 'django.contrib.auth.password_validation.CommonPasswordValidator', },
    { 'NAME': 'django.contrib.auth.password_validation.NumericPasswordValidator', },
]

# Internationalization
LANGUAGE_CODE = 'es-es'
TIME_ZONE = 'UTC'
USE_I18N = True
USE_L10N  = True
USE_TZ = True


STATIC_URL = 'static/'
STATICFILES_DIRS = [
    os.path.join(BASE_DIR, 'GARCA', 'static'),
]
STATIC_ROOT = os.path.join(BASE_DIR, 'staticfiles')

DEFAULT_AUTO_FIELD = 'django.db.models.BigAutoField'

# Media files
MEDIA_ROOT = BASE_DIR / 'media'
MEDIA_URL = '/media/'

ADMIN_SITE_TITLE = 'GARCA'
ADMIN_SITE_HEADER = 'GARCA'

# Configuración de Celery
CELERY_BROKER_URL = 'redis://localhost:6379/0'
CELERY_RESULT_BACKEND = 'redis://localhost:6379/0'
CELERY_ACCEPT_CONTENT = ['json']
CELERY_TASK_SERIALIZER = 'json'
CELERY_RESULT_SERIALIZER = 'json'
CELERY_TIMEZONE = 'UTC'

SECRET_KEY = 'django-insecure-u4ih8i#^0=x+j4+*b(7uwf-3m-!jo4e!!p+pww5sns#69g2_j-'
DEBUG = True
ALLOWED_HOSTS = ['*', 'localhost', '127.0.0.1'] # Permite cualquier host en desarrollo


DATABASES = {
    'default': {
        'ENGINE': 'django.db.backends.sqlite3',
        'NAME': BASE_DIR / 'garca.sqlite3', # <-- Base de datos para producción
    }
    # Ejemplo para PostgreSQL en producción (usar variables de entorno para credenciales):
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
    'formatters': {
        'verbose': {
            'format': '{levelname} {asctime} {module} {process:d} {thread:d} {message}',
            'style': '{',
        },
        'simple': {
            'format': '{levelname} {message}',
            'style': '{',
        },
    },
    'handlers': {
        'console': {
            'class': 'logging.StreamHandler', # Puedes usar 'verbose' para más detalle
            'formatter': 'simple',
        },
    },
    'root': {
        'handlers': ['console'],
        'level': 'WARNING',
    },
    'loggers': {
        'bank_imports': {
            'handlers': ['console'],
            'level': 'DEBUG', # Cambia a DEBUG para ver los warnings de filas saltadas
            'propagate': False, # Evita que el mensaje se envíe también al root logger si ya lo manejaste aquí
        },
        'django': {
            'handlers': ['console'],
            'level': os.getenv('DJANGO_LOG_LEVEL', 'INFO'),
            'propagate': False,
        },
    },
}

CACHES = {
    'default': {
        'BACKEND': 'django.core.cache.backends.locmem.LocMemCache',
        'LOCATION': 'unique-snowflake', 
    }
}

# --- Configuración para Google Sheets ---
GOOGLE_CREDENTIALS_FILE_PATH = BASE_DIR / 'garca-google-credentials.json' # Asegúrate que este archivo exista en la raíz del proyecto
GOOGLE_DRIVE_BIARCA_FOLDER_ID = '1DaQKgdiDRiAYee2z-dUB_Wzs_t5Za1ir' # ¡Reemplaza esto con el ID real de tu carpeta BIARCA en Google Drive!
GOOGLE_SHEET_NAME = 'BIData'
# GOOGLE_SHEET_ID es el ID de la hoja de cálculo específica. Si se crea una nueva, este ID debe actualizarse.
GOOGLE_SHEET_ID = '1nK1amy9FPnlBhBZu1yLCHTrHqUrHvWyj7AMUp7B90bc'
GOOGLE_SHEET_WORKSHEET_NAME = 'Movimientos'
