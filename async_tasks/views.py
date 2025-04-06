from django.shortcuts import render
from django.http import JsonResponse
from .tasks import hello_world, recalculate_balances_after_date, calculate_inicial_balances

def trigger_hello_world(request):
    # Ejecutar tarea asíncrona
    task = hello_world.delay()
    return JsonResponse({
        'success': True,
        'task_id': task.id,
        'message': 'Tarea iniciada correctamente'
    })

def trigger_calculate_balance_after_date(request,date, account_id):
    # Ejecutar tarea asíncrona
    task = recalculate_balances_after_date(date, account_id).delay()
    return JsonResponse({
        'success': True,
        'task_id': task.id,
        'message': 'Tarea iniciada correctamente'
    })

def trigger_calculate_inicial_balances(request):
    # Ejecutar tarea asíncrona
    task = calculate_inicial_balances.delay()
    return JsonResponse({
        'success': True,
        'task_id': task.id,
        'message': 'Tarea iniciada correctamente'
    })
    
