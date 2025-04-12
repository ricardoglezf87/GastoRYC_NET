import subprocess
import tkinter as tk
from tkinter import scrolledtext
import threading
from pystray import Icon, Menu, MenuItem
from PIL import Image

# Variables globales para los procesos
redis_process = None
celery_process = None
django_process = None

def iniciar_redis():
    global redis_process
    redis_process = subprocess.Popen(["async_tasks\\redis\\redis-server.exe"], stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    mostrar_salida(redis_text, "Redis iniciado")

def iniciar_celery():
    global celery_process
    celery_process = subprocess.Popen(["celery", "-A", "GARCA", "worker", "-l", "info", "--pool=solo"], stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    mostrar_salida(celery_text, "Celery iniciado")

def iniciar_django():
    global django_process
    django_process = subprocess.Popen(["python", "manage.py", "runserver"], stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    mostrar_salida(django_text, "Django iniciado")

def detener_redis():
    global redis_process
    if redis_process:
        redis_process.terminate()
        mostrar_salida(redis_text, "Redis detenido")

def detener_celery():
    global celery_process
    if celery_process:
        celery_process.terminate()
        mostrar_salida(celery_text, "Celery detenido")

def detener_django():
    global django_process
    if django_process:
        django_process.terminate()
        django_process.kill()
        mostrar_salida(django_text, "Django detenido")

def mostrar_salida(text_widget, mensaje):
    text_widget.insert(tk.END, mensaje + "\n")
    text_widget.see(tk.END)

def ejecutar_proceso(proceso, text_widget):
    while True:
        line = proceso.stdout.readline()
        if not line:
            break
        mostrar_salida(text_widget, line.decode("utf-8").strip())

def crear_interfaz():
    global redis_text, celery_text, django_text
    ventana = tk.Tk()
    ventana.title("Gestor de Procesos")

    redis_frame = tk.Frame(ventana)
    redis_frame.pack(pady=10)
    tk.Label(redis_frame, text="Redis:").pack()
    redis_text = scrolledtext.ScrolledText(redis_frame, width=60, height=5)
    redis_text.pack()
    tk.Button(redis_frame, text="Iniciar Redis", command=iniciar_redis).pack()
    tk.Button(redis_frame, text="Detener Redis", command=detener_redis).pack()

    celery_frame = tk.Frame(ventana)
    celery_frame.pack(pady=10)
    tk.Label(celery_frame, text="Celery:").pack()
    celery_text = scrolledtext.ScrolledText(celery_frame, width=60, height=5)
    celery_text.pack()
    tk.Button(celery_frame, text="Iniciar Celery", command=iniciar_celery).pack()
    tk.Button(celery_frame, text="Detener Celery", command=detener_celery).pack()

    django_frame = tk.Frame(ventana)
    django_frame.pack(pady=10)
    tk.Label(django_frame, text="Django:").pack()
    django_text = scrolledtext.ScrolledText(django_frame, width=60, height=5)
    django_text.pack()
    tk.Button(django_frame, text="Iniciar Django", command=iniciar_django).pack()
    tk.Button(django_frame, text="Detener Django", command=detener_django).pack()
    return ventana

def ocultar_ventana():
    ventana.withdraw()
    image = Image.open(".\\GARCA\\static\\img\\logo.png") #Asegurate de tener un icono llamado icono.png en el mismo directorio.
    menu = Menu(MenuItem("Mostrar", mostrar_ventana), MenuItem("Salir", salir))
    icono = Icon("nombre", image, "titulo", menu)
    icono.run()

def mostrar_ventana(icono, item):
    ventana.deiconify()
    icono.stop()

def salir(icono, item):
    detener_redis()
    detener_celery()
    detener_django()
    icono.stop()
    ventana.destroy()

ventana = crear_interfaz()

ventana.protocol("WM_DELETE_WINDOW", ocultar_ventana)

ventana.mainloop()