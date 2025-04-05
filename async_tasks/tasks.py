from celery import shared_task

@shared_task(bind=True)
def hello_world(self):
    print("¡Hola Mundo!")
    return "¡Hola Mundo!"