# reports/forms.py
from django import forms
from datetime import date, timedelta
from dateutil.relativedelta import relativedelta

PERIOD_CHOICES = [
    ('all', 'Todo'),
    ('current_month', 'Mes Actual'),
    ('last_month', 'Mes Anterior'),
    ('last_30_days', 'Últimos 30 días'),
    ('last_60_days', 'Últimos 60 días'),
    ('current_year', 'Año Actual'),
    ('last_year', 'Año Anterior'),
    # Añade más periodos si es necesario
]

class PeriodFilterForm(forms.Form):
    period = forms.ChoiceField(choices=PERIOD_CHOICES, required=False, initial='current_month', label="Periodo")
    # Eliminamos start_date y end_date
    # start_date = forms.DateField(widget=forms.DateInput(attrs={'type': 'date'}), required=False, label="Fecha Inicio")
    # end_date = forms.DateField(widget=forms.DateInput(attrs={'type': 'date'}), required=False, label="Fecha Fin")

    # El método clean ahora solo necesita calcular basado en 'period'
    def clean(self):
        cleaned_data = super().clean()
        period = cleaned_data.get('period')
        start_date = None
        end_date = None

        # Calcular fechas basado en el periodo seleccionado
        if period and period != 'all':
            today = date.today()
            if period == 'current_month':
                start_date = today.replace(day=1)
                end_date = today # O podrías poner fin de mes: today.replace(day=1) + relativedelta(months=1) - timedelta(days=1)
            elif period == 'last_month':
                end_of_last_month = today.replace(day=1) - timedelta(days=1)
                start_date = end_of_last_month.replace(day=1)
                end_date = end_of_last_month
            elif period == 'last_30_days':
                 start_date = today - timedelta(days=30)
                 end_date = today
            elif period == 'last_60_days':
                 start_date = today - timedelta(days=60)
                 end_date = today
            elif period == 'current_year':
                 start_date = today.replace(month=1, day=1)
                 end_date = today
            elif period == 'last_year':
                 last_year_end = today.replace(month=1, day=1) - timedelta(days=1)
                 start_date = last_year_end.replace(month=1, day=1)
                 end_date = last_year_end
            # Añadir lógica para otros periodos

        # Guardamos las fechas calculadas (o None si es 'all') en cleaned_data
        # para que get_date_range las pueda usar.
        cleaned_data['calculated_start_date'] = start_date
        cleaned_data['calculated_end_date'] = end_date

        return cleaned_data

    def get_date_range(self):
        # Asegurarse de llamar a clean() primero si no se ha hecho
        if not hasattr(self, 'cleaned_data'):
             self.is_valid() # Llama a clean() internamente

        # Devolver las fechas calculadas guardadas en clean()
        return self.cleaned_data.get('calculated_start_date'), self.cleaned_data.get('calculated_end_date')

