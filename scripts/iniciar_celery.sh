#!/bin/bash

# Configura las rutas
DJANGO_DIR="/volume1/web/GastoRYC_NET"
LOG_FILE="/volume1/web/GastoRYC_NET/logs/celery_server.log"
PID_FILE="/volume1/web/GastoRYC_NET/logs/celery_server.pid"

# Ir al directorio del proyecto
cd "$DJANGO_DIR" || exit 1

# Ejecutar el servidor Django en segundo plano y guardar PID
nohup python3.9 -m celery -A GARCA worker -l info --pool=solo  > "$LOG_FILE" 2>&1 &

# Guardar el PID del proceso
echo $! > "$PID_FILE"

echo "Servidor iniciado. PID guardado en $PID_FILE"