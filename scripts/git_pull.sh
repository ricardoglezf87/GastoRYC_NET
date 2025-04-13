#!/bin/bash

# Configura las rutas
DJANGO_DIR="/volume1/web/GastoRYC_NET"
LOG_FILE="/volume1/web/GastoRYC_NET/logs/git_pull.log"

# Ir al directorio del proyecto
cd "$DJANGO_DIR" || exit 1

git pull  > "$LOG_FILE" 2>&1 &

echo "Git pull realizado."