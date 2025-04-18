from django import forms
from .models import Attachment

class AttachmentForm(forms.ModelForm):
    class Meta:
        model = Attachment
        fields = ['file', 'description']

class MultiplePdfUploadForm(forms.Form): # Mantenemos el nombre por ahora
    # Cambiamos el campo para que sea un solo archivo
    pdf_file = forms.FileField( # Renombrado de pdf_files a pdf_file
        label='Selecciona un archivo PDF para asociar',
        # Quitamos el widget con multiple=True
        widget=forms.FileInput(), # Widget estándar para un archivo
        help_text='El sistema intentará encontrar el asiento correspondiente para este PDF.',
        required=True
    )