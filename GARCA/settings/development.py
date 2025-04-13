# GARCA/settings/development.py
from .base import * # Importa toda la configuración base

# SECURITY WARNING: keep the secret key used in production secret!
# Puedes usar una clave simple para desarrollo
SECRET_KEY = 'django-insecure-u4ih8i#^0=x+j4+*b(7uwf-3m-!jo4e!!p+pww5sns#69g2_j-'

# SECURITY WARNING: don't run with debug turned on in production!
DEBUG = True

ALLOWED_HOSTS = ['*', 'localhost', '127.0.0.1'] # Permite cualquier host en desarrollo

# Database para Desarrollo
DATABASES = {
    'default': {
        'ENGINE': 'django.db.backends.sqlite3',
        'NAME': BASE_DIR / 'db.sqlite3', # <-- Base de datos para desarrollo
    }
}

print("--- Cargando configuración de DESARROLLO ---") # Para confirmar qué se carga
