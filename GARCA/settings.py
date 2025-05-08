# GARCA/settings.py
"""
Configuración base de Django para GARCA project.
"""

import os
from pathlib import Path

# Build paths inside the project like this: BASE_DIR / 'subdir'.
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
        # Asegúrate que la ruta a las plantillas base sea correcta
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

# Cambia la referencia a la carpeta 'settings'
WSGI_APPLICATION = 'GARCA.wsgi.application'
ASGI_APPLICATION = 'GARCA.asgi.application' # Añade esto si no lo tenías


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


# Static files (CSS, JavaScript, Images)

STATIC_URL = 'static/'
STATICFILES_DIRS = [
    os.path.join(BASE_DIR, 'GARCA', 'static'),
]
STATIC_ROOT = os.path.join(BASE_DIR, 'staticfiles')


# Default primary key field type
DEFAULT_AUTO_FIELD = 'django.db.models.BigAutoField'

# Media files (si los usas para adjuntos, mantenlo)
MEDIA_ROOT = BASE_DIR / 'media' # Es mejor usar una subcarpeta 'media'
MEDIA_URL = '/media/'           # Añade la URL para acceder a ellos

ADMIN_SITE_TITLE = 'GARCA'
ADMIN_SITE_HEADER = 'GARCA'

# Configuración de Celery (puede quedarse aquí si es la misma para ambos)
CELERY_BROKER_URL = 'redis://localhost:6379/0'
CELERY_RESULT_BACKEND = 'redis://localhost:6379/0'
CELERY_ACCEPT_CONTENT = ['json']
CELERY_TASK_SERIALIZER = 'json'
CELERY_RESULT_SERIALIZER = 'json'
CELERY_TIMEZONE = 'UTC' # O tu zona horaria

# SECURITY WARNING: keep the secret key used in production secret!
# Puedes usar una clave simple para desarrollo
SECRET_KEY = 'django-insecure-u4ih8i#^0=x+j4+*b(7uwf-3m-!jo4e!!p+pww5sns#69g2_j-'

# SECURITY WARNING: don't run with debug turned on in production!
DEBUG = True

ALLOWED_HOSTS = ['*', 'localhost', '127.0.0.1'] # Permite cualquier host en desarrollo


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
            'class': 'logging.StreamHandler',
            'formatter': 'simple', # Puedes usar 'verbose' para más detalle
        },
    },
    'root': {
        'handlers': ['console'],
        'level': 'WARNING', # Nivel por defecto para todos los loggers
    },
    'loggers': {
        'bank_imports': { # Logger específico para tu app bank_imports
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
