@echo off
REM --- Configuración Opcional ---
REM Cambia al directorio donde está launcher.py si ejecutas el .bat desde otro lugar
REM cd /d "C:\ruta\a\tu\proyecto\GastoRYC_NET"

REM Activa tu entorno virtual si usas uno (ajusta la ruta si es necesario)
REM .\venv\Scripts\activate
REM O si usas conda:
REM call conda activate tu_entorno

REM --- Ejecución ---
echo Iniciando GARCA Launcher en segundo plano...
REM Usa "start" para lanzar el proceso de forma independiente
REM Usa "pythonw.exe" para evitar la ventana de consola de Python
REM Añade el argumento "--hidden" que hemos creado en launcher.py
start "" pythonw.exe launcher.py --hidden

echo.
echo El lanzador deberia estar ejecutandose y visible solo en la bandeja del sistema.
REM Puedes quitar la pausa si no quieres que esta ventana de .bat permanezca abierta
REM pause

exit /b 0
