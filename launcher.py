import subprocess
import tkinter as tk
from tkinter import scrolledtext
import threading
import queue
import os
import signal

try:
    from pystray import Icon as SysTrayIcon, Menu, MenuItem
    from PIL import Image
    PYSTRAY_AVAILABLE = True
except ImportError:
    PYSTRAY_AVAILABLE = False
    print("Advertencia: pystray o Pillow no están instalados. La funcionalidad de icono de bandeja no estará disponible.")
    print("Instálalos con: pip install pystray Pillow")

redis_process = None
celery_process = None
django_process = None
redis_thread = None
celery_thread = None
django_thread = None
tray_icon = None # Para mantener referencia al objeto Icon
gui_queue = queue.Queue()


def iniciar_proceso(comando, text_widget, nombre_proceso):
    """Inicia un proceso y un hilo para leer su salida."""
    global redis_process, celery_process, django_process
    global redis_thread, celery_thread, django_thread

    try:
        startupinfo = None
        creationflags = 0
        if os.name == 'nt':
            startupinfo = subprocess.STARTUPINFO()
            startupinfo.dwFlags |= subprocess.STARTF_USESHOWWINDOW
            startupinfo.wShowWindow = subprocess.SW_HIDE
            creationflags = subprocess.CREATE_NEW_PROCESS_GROUP

        proceso = subprocess.Popen(
            comando,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            text=True,
            encoding='utf-8',
            errors='replace',
            bufsize=1,
            universal_newlines=True,
            startupinfo=startupinfo,
            creationflags=creationflags
        )

        if nombre_proceso == "Redis":
            redis_process = proceso
        elif nombre_proceso == "Celery":
            celery_process = proceso
        elif nombre_proceso == "Django":
            django_process = proceso

        hilo = threading.Thread(target=leer_salida_proceso, args=(proceso, text_widget, nombre_proceso), daemon=True)

        if nombre_proceso == "Redis":
            redis_thread = hilo
        elif nombre_proceso == "Celery":
            celery_thread = hilo
        elif nombre_proceso == "Django":
            django_thread = hilo

        hilo.start()
        gui_queue.put((text_widget, f"{nombre_proceso} iniciado (PID: {proceso.pid})...\n"))
        return proceso, hilo

    except FileNotFoundError:
        gui_queue.put((text_widget, f"Error: Comando '{comando[0]}' no encontrado. Asegúrate de que está en el PATH o la ruta es correcta.\n"))
    except Exception as e:
        gui_queue.put((text_widget, f"Error al iniciar {nombre_proceso}: {e}\n"))
    return None, None


def detener_proceso(proceso, text_widget, nombre_proceso):
    if proceso and proceso.poll() is None:
        try:
            if os.name == 'nt':
                proceso.send_signal(signal.CTRL_BREAK_EVENT)
            else:
                proceso.terminate()
            proceso.wait(timeout=5)
            gui_queue.put((text_widget, f"{nombre_proceso} detenido.\n"))
        except subprocess.TimeoutExpired:
            gui_queue.put((text_widget, f"{nombre_proceso} no respondió, forzando cierre (kill)...\n"))
            proceso.kill()
            proceso.wait()
            gui_queue.put((text_widget, f"{nombre_proceso} forzado a detenerse.\n"))
        except Exception as e:
             gui_queue.put((text_widget, f"Error al detener {nombre_proceso}: {e}\n"))
        finally:
            if nombre_proceso == "Redis":
                global redis_process
                redis_process = None
            elif nombre_proceso == "Celery":
                global celery_process
                celery_process = None
            elif nombre_proceso == "Django":
                global django_process
                django_process = None
    else:
        pass # No hacer nada si el proceso no estaba corriendo


def leer_salida_proceso(proceso, text_widget, nombre_proceso):
    try:
        for linea in iter(proceso.stdout.readline, ''):
            if not linea:
                 break
            gui_queue.put((text_widget, linea))
    except Exception as e:
        gui_queue.put((text_widget, f"Error leyendo salida de {nombre_proceso}: {e}\n"))
    finally:
        if proceso.stdout:
            proceso.stdout.close()
        exit_code = proceso.poll()
        gui_queue.put((text_widget, f"--- {nombre_proceso} terminado (Código: {exit_code}) ---\n"))


def iniciar_redis():
    if redis_process and redis_process.poll() is None:
        gui_queue.put((redis_text, "Redis ya está iniciado.\n"))
        return
    redis_path = os.path.join("async_tasks", "redis", "redis-server.exe")
    if not os.path.exists(redis_path):
         gui_queue.put((redis_text, f"Error: No se encontró {redis_path}\n"))
         return
    iniciar_proceso([redis_path], redis_text, "Redis")

def iniciar_celery():
    if celery_process and celery_process.poll() is None:
        gui_queue.put((celery_text, "Celery ya está iniciado.\n"))
        return
    iniciar_proceso(["celery", "-A", "GARCA", "worker", "-l", "info", "--pool=solo"], celery_text, "Celery")

def iniciar_django():
    if django_process and django_process.poll() is None:
        gui_queue.put((django_text, "Django ya está iniciado.\n"))
        return
    iniciar_proceso(["python", "manage.py", "runserver","8585", "--noreload"], django_text, "Django")

def detener_redis():
    detener_proceso(redis_process, redis_text, "Redis")

def detener_celery():
    detener_proceso(celery_process, celery_text, "Celery")

def detener_django():
    detener_proceso(django_process, django_text, "Django")

def mostrar_salida(text_widget, mensaje):
    text_widget.configure(state='normal') # Habilitar escritura
    text_widget.insert(tk.END, mensaje)
    text_widget.see(tk.END) # Mueve el scroll al final
    text_widget.configure(state='disabled') # Deshabilitar escritura para evitar edición manual

def procesar_cola_gui():
    try:
        while True:
            widget, mensaje = gui_queue.get_nowait()
            mostrar_salida(widget, mensaje)
    except queue.Empty:
        pass
    ventana.after(100, procesar_cola_gui) # Volver a llamar después de 100ms

def crear_interfaz():
    global redis_text, celery_text, django_text, ventana
    ventana = tk.Tk()
    ventana.title("Gestor de Procesos GARCA")
    ventana.geometry("700x550")

    redis_frame = tk.LabelFrame(ventana, text="Redis", padx=10, pady=5)
    redis_frame.pack(pady=10, padx=10, fill="x")
    redis_text = scrolledtext.ScrolledText(redis_frame, width=80, height=6, wrap=tk.WORD, state='disabled')
    redis_text.pack(pady=5, fill="x", expand=True)
    btn_frame_redis = tk.Frame(redis_frame)
    btn_frame_redis.pack()
    tk.Button(btn_frame_redis, text="Iniciar Redis", command=iniciar_redis).pack(side=tk.LEFT, padx=5)
    tk.Button(btn_frame_redis, text="Detener Redis", command=detener_redis).pack(side=tk.LEFT, padx=5)

    celery_frame = tk.LabelFrame(ventana, text="Celery Worker", padx=10, pady=5)
    celery_frame.pack(pady=10, padx=10, fill="x")
    celery_text = scrolledtext.ScrolledText(celery_frame, width=80, height=6, wrap=tk.WORD, state='disabled')
    celery_text.pack(pady=5, fill="x", expand=True)
    btn_frame_celery = tk.Frame(celery_frame)
    btn_frame_celery.pack()
    tk.Button(btn_frame_celery, text="Iniciar Celery", command=iniciar_celery).pack(side=tk.LEFT, padx=5)
    tk.Button(btn_frame_celery, text="Detener Celery", command=detener_celery).pack(side=tk.LEFT, padx=5)

    django_frame = tk.LabelFrame(ventana, text="Django Server", padx=10, pady=5)
    django_frame.pack(pady=10, padx=10, fill="x")
    django_text = scrolledtext.ScrolledText(django_frame, width=80, height=6, wrap=tk.WORD, state='disabled')
    django_text.pack(pady=5, fill="x", expand=True)
    btn_frame_django = tk.Frame(django_frame)
    btn_frame_django.pack()
    tk.Button(btn_frame_django, text="Iniciar Django", command=iniciar_django).pack(side=tk.LEFT, padx=5)
    tk.Button(btn_frame_django, text="Detener Django", command=detener_django).pack(side=tk.LEFT, padx=5)

    return ventana

def ocultar_a_bandeja():
    """Oculta la ventana principal y muestra el icono en la bandeja."""
    if not PYSTRAY_AVAILABLE:
        print("Funcionalidad de bandeja no disponible. Saliendo...")
        salir_aplicacion()
        return

    global tray_icon
    if tray_icon and tray_icon.visible: # Evitar crear múltiples iconos
        ventana.withdraw() # Solo ocultar si el icono ya existe
        return

    ventana.withdraw()
    try:
        # Asegúrate que la ruta al logo es correcta RELATIVA a launcher.py
        base_path = os.path.dirname(os.path.abspath(__file__))
        image_path = os.path.join(base_path, "GARCA", "static", "img", "logo.png")
        image = Image.open(image_path)

        menu = Menu(
            MenuItem("Mostrar", mostrar_desde_bandeja, default=True),
            Menu.SEPARATOR,
            MenuItem("Salir", salir_aplicacion)
        )
        tray_icon = SysTrayIcon("GARCA Launcher", image, "GARCA Launcher", menu)
        threading.Thread(target=tray_icon.run, daemon=True).start()
        print("Aplicación oculta en la bandeja del sistema.")
    except FileNotFoundError:
        print(f"Error: No se encontró el icono en {image_path}")
        print("La aplicación no se ocultará en la bandeja.")
        mostrar_ventana_emergencia() # Mostrarla de nuevo si falla el icono
    except Exception as e:
        print(f"Error al crear el icono de bandeja: {e}")
        mostrar_ventana_emergencia()

def mostrar_desde_bandeja():
    """Muestra la ventana principal y detiene el icono de bandeja."""
    global tray_icon
    if tray_icon:
        print("Mostrando ventana...")
        tray_icon.stop()
        tray_icon = None
    mostrar_ventana_emergencia()

def mostrar_ventana_emergencia():
    try:
        if ventana.state() == 'withdrawn':
            ventana.deiconify()
        ventana.lift()
        ventana.focus_force()
    except tk.TclError:
        print("La ventana ya no existe.")

def salir_aplicacion():
    print("Saliendo...")
    global tray_icon
    if tray_icon:
        tray_icon.stop()
        tray_icon = None

    # Detener procesos (en orden inverso de dependencia si importa)
    detener_django()
    detener_celery()
    detener_redis()
    ventana.after(500, ventana.destroy)


if __name__ == "__main__":
    ventana = crear_interfaz()

    if PYSTRAY_AVAILABLE:
        ventana.protocol("WM_DELETE_WINDOW", ocultar_a_bandeja)
    else:
        ventana.protocol("WM_DELETE_WINDOW", salir_aplicacion)

    procesar_cola_gui()
    ventana.mainloop()

    print("Ventana cerrada.")
    # Asegurarse de que los procesos se detienen si la ventana se cierra inesperadamente
    if redis_process and redis_process.poll() is None: detener_redis()
    if celery_process and celery_process.poll() is None: detener_celery()
    if django_process and django_process.poll() is None: detener_django()
