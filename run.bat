@echo off

REM Abre una nueva ventana para ejecutar el servidor de Redis
start "Redis Server" cmd /k "async_tasks\redis\redis-server.exe"

REM Abre otra nueva ventana para ejecutar el worker de Celery
start "Celery Worker" cmd /k "celery -A GARCA worker -l info --pool=solo"

REM Abre una tercera ventana para ejecutar el servidor de Django
start "Django Server" cmd /k "python manage.py runserver"

echo Se han iniciado Redis, Celery Worker y el Servidor de Django en ventanas separadas.
pause