from django.shortcuts import render
from django.http import JsonResponse
from .tasks import hello_world

def trigger_hello_world(request):
    # Ejecutar tarea asíncrona
    task = hello_world.delay()
    return JsonResponse({
        'success': True,
        'task_id': task.id,
        'message': 'Tarea iniciada correctamente'
    })

# Create your views here.
