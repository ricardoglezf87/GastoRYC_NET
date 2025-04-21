from django.shortcuts import get_object_or_404
from django.http import JsonResponse
from django.views.decorators.http import require_POST
from django.views.decorators.csrf import csrf_exempt
from django.contrib.contenttypes.models import ContentType
from .models import Attachment
from accounts.models import Account
from entries.models import Entry
import json

@csrf_exempt
def upload_attachments(request, account_id):
    if request.method == 'POST':
        account = get_object_or_404(Account, id=account_id)
        for file in request.FILES.getlist('files'):
            Attachment.objects.create(
                file=file,
                content_type=ContentType.objects.get_for_model(Account),
                description=file.name,
                object_id=account.id
            )
        return JsonResponse({'success': True})
    return JsonResponse({'success': False})

@csrf_exempt
def upload_entry_attachments(request, entry_id):
    if request.method == 'POST':
        entry = get_object_or_404(Entry, id=entry_id)
        for file in request.FILES.getlist('files'):
            Attachment.objects.create(
                file=file,
                content_type=ContentType.objects.get_for_model(Entry),
                description=file.name,
                object_id=entry.id
            )
        return JsonResponse({'success': True})
    return JsonResponse({'success': False})

@require_POST
@csrf_exempt
def update_attachment_description(request, attachment_id):
    attachment = get_object_or_404(Attachment, id=attachment_id)
    data = json.loads(request.body)
    attachment.description = data.get('description', '')
    attachment.save()
    return JsonResponse({'success': True})

@require_POST
@csrf_exempt
def delete_attachment(request, attachment_id):
    attachment = get_object_or_404(Attachment, id=attachment_id)
    attachment.delete()
    return JsonResponse({'success': True})