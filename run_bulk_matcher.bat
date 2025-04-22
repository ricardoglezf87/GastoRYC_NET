@echo off
REM --- Configuración Opcional ---
REM Cambia al directorio donde está tu proyecto si ejecutas el .bat desde otro lugar
REM cd /d "C:\ruta\a\tu\proyecto\GastoRYC_NET"

REM Activa tu entorno virtual si usas uno (ajusta la ruta si es necesario)
REM .\venv\Scripts\activate
REM O si usas conda:
REM call conda activate tu_entorno

REM --- Ejecución ---
echo Iniciando GARCA bulk_matcher_app
REM Usa "start" para lanzar el proceso de forma independiente
REM Usa "pythonw.exe" para evitar la ventana de consola de Python (si es una app GUI o de fondo)
REM Si bulk_matcher_app necesita una consola visible, usa "python.exe" en lugar de "pythonw.exe"
REM Usa "-m" para ejecutar el módulo Python especificado
python -m document_classified.bulk_matcher_app

echo.
echo El lanzador bulk_matcher_app deberia estar ejecutandose.
REM Puedes quitar la pausa si no quieres que esta ventana de .bat permanezca abierta
REM pause

exit /b 0
