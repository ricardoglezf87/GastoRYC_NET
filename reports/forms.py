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
        ('last_5_years', 'Últimos 5 Años'), 
        ('all', 'Todas las Fechas'), # Añadimos opción para todo
    ]

    period = forms.ChoiceField(
        choices=PERIOD_CHOICES,
        required=False,
        label="Periodo Predefinido",
        widget=forms.Select(attrs={'class': 'form-control'}) # Asegúrate de que tenga clases de Bootstrap si es necesario
    )

    def clean(self):
        cleaned_data = super().clean()
        # La validación de periodo se maneja por las opciones del ChoiceField.
        # No se necesita validación adicional aquí ya que 'start_date' y 'end_date'
        # fueron eliminados.
        return cleaned_data

    def get_date_range(self):
        cleaned_data = self.cleaned_data
        period = cleaned_data.get('period')
        today = date.today()

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
        else: # Caso por defecto (selección vacía inicial o inválida)
            return None, None
