# reports/forms.py (o donde esté tu formulario)
from django import forms
from datetime import date, timedelta
from dateutil.relativedelta import relativedelta # Necesitarás instalar: pip install python-dateutil

class PeriodFilterForm(forms.Form):
    # Opciones de periodo predefinidas
    PERIOD_CHOICES = [
        ('', '---------'), # Opción vacía o por defecto
        ('today', 'Hoy'),
        ('this_week', 'Esta Semana'),
        ('this_month', 'Este Mes'),
        ('last_month', 'Mes Pasado'),
        ('this_year', 'Este Año'),        
        ('last_year', 'Año Pasado'),
        ('last_3_years', 'Últimos 3 Años'), # Nueva opción
        ('last_5_years', 'Últimos 5 Años'), # Nueva opción
        # ('custom', 'Personalizado...'), # Eliminamos la opción personalizada
        ('all', 'Todas las Fechas'), # Añadimos opción para todo
    ]

    period = forms.ChoiceField(
        choices=PERIOD_CHOICES,
        required=False,
        label="Periodo Predefinido",
        widget=forms.Select(attrs={'class': 'form-control'}) # Asegúrate de que tenga clases de Bootstrap si es necesario
    )
    # Eliminamos los campos de fecha ya que no hay opción personalizada
    # start_date = forms.DateField(
    #     required=False,
    #     widget=forms.DateInput(attrs={'type': 'date', 'class': 'form-control'}),
    #     label="Fecha Inicio"
    # )
    # end_date = forms.DateField(
    #     required=False,
    #     widget=forms.DateInput(attrs={'type': 'date', 'class': 'form-control'}),
    #     label="Fecha Fin"
    # )

    def clean(self):
        cleaned_data = super().clean()
        period = cleaned_data.get('period')
        start_date = cleaned_data.get('start_date')
        end_date = cleaned_data.get('end_date')

        # Eliminamos la validación para 'custom'
        # if period == 'custom' and not (start_date and end_date):
        #     raise forms.ValidationError(
        #         "Si seleccionas 'Personalizado', debes especificar Fecha Inicio y Fecha Fin."
        #     )

        # Eliminamos la validación de fechas ya que no existen los campos
        # if start_date and end_date and start_date > end_date:
        #     raise forms.ValidationError("La Fecha Inicio no puede ser posterior a la Fecha Fin.")

        # Podrías añadir lógica aquí para calcular start/end date basado en 'period'
        # si no quieres hacerlo en la vista, pero generalmente es más claro en la vista.

        return cleaned_data

    # Función auxiliar (opcional, podrías tenerla en la vista)
    # para obtener el rango de fechas basado en la selección del periodo
    def get_date_range(self):
        cleaned_data = self.cleaned_data
        period = cleaned_data.get('period')
        # Ya no necesitamos obtener start_date/end_date del form
        # start_date = cleaned_data.get('start_date')
        # end_date = cleaned_data.get('end_date')
        today = date.today()

        # Eliminamos la lógica para 'custom'
        # if (period == 'custom' or not period) and start_date and end_date:
        #      # Asegurarse de que end_date incluya todo el día si es necesario (depende de tu lógica de filtrado)
        #      # Si filtras por fecha exacta, esto está bien. Si es un rango, podrías necesitar ajustar.
        #     return start_date, end_date
        if period == 'today':
            return today, today
        elif period == 'this_week':
            start = today - timedelta(days=today.weekday())
            end = start + timedelta(days=6)
            return start, end
        elif period == 'this_month':
            start = today.replace(day=1)
            end = (start + relativedelta(months=1)) - timedelta(days=1)
            return start, end
        elif period == 'this_year':
            start = today.replace(day=1, month=1)
            end = today.replace(day=31, month=12)
            return start, end
        elif period == 'last_month':
            end = today.replace(day=1) - timedelta(days=1)
            start = end.replace(day=1)
            return start, end
        elif period == 'last_year':
            start = today.replace(day=1, month=1, year=today.year - 1)
            end = start.replace(day=31, month=12)
            return start, end
        elif period == 'last_3_years':
            start = today.replace(year=today.year - 3) + timedelta(days=1) # Desde hace 3 años hasta hoy
            return start, today
        elif period == 'last_5_years':
            start = today.replace(year=today.year - 5) + timedelta(days=1) # Desde hace 5 años hasta hoy
            return start, today
        elif period == 'all': # Nueva opción para todas las fechas
            return None, None # Indica que no hay filtro de fecha
        else: # Caso por defecto (selección vacía inicial)
            # Caso por defecto o si no hay selección válida, quizás devolver None o un rango por defecto
            return None, None
