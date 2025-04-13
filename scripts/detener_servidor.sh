!/bin/bash

# Ruta del archivo PID
PID_FILE="/volume1/web/GastoRYC_NET/logs/django_server.pid"

if [ ! -f "$PID_FILE" ]; then
  echo "No se encontró el archivo PID."
  exit 1
fi

PID=$(cat "$PID_FILE")

if ps -p $PID > /dev/null; then
  kill $PID
  echo "Servidor detenido (PID: $PID)."
else
  echo "No se encontró ningún proceso con PID: $PID"
fi

# Eliminar el archivo PID
rm -f "$PID_FILE"
