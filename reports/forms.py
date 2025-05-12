from django import forms
from datetime import date, timedelta
from dateutil.relativedelta import relativedelta # pip install python-dateutil

class PeriodFilterForm(forms.Form):
    PERIOD_CHOICES = [
        ('', '---------'), # Opción vacía o por defecto
        ('today', 'Hoy'),
        ('this_week', 'Esta Semana'),
        ('this_month', 'Este Mes'),
        ('last_month', 'Mes Pasado'),
        ('this_year', 'Este Año'),        
        ('last_year', 'Año Pasado'),
        ('last_3_years', 'Últimos 3 Años'),
        ('last_5_years', 'Últimos 5 Años'), 
        ('all', 'Todas las Fechas'),
    ]

    period = forms.ChoiceField(
        choices=PERIOD_CHOICES,
        required=False,
        label="Periodo Predefinido",
        widget=forms.Select(attrs={'class': 'form-control'}) # Asegúrate de que tenga clases de Bootstrap si es necesario
    )

    def clean(self):
        cleaned_data = super().clean()
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
            start = today.replace(year=today.year - 3) + timedelta(days=1)
            return start, today
        elif period == 'last_5_years':
            start = today.replace(year=today.year - 5) + timedelta(days=1)
            return start, today
        elif period == 'all':
            return None, None # Indica que no se aplica filtro de fecha
        else: # Caso por defecto o selección vacía
            return None, None
