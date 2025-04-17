from django import template
from django.utils.safestring import mark_safe
from django.utils import formats
from django.conf import settings

register = template.Library()

NON_BREAKING_SPACE = '\xa0'
DESIRED_THOUSAND_SEPARATOR = '.'

@register.simple_tag
def generate_account_tree(accounts,show_closed):
    def generate_tree_html(accounts):
        html = ''
        for account in accounts:

            if account.closed and not show_closed:
                continue

            balance = account.get_balance()
            formatted_balance = "" # Inicializar

            try:
                if settings.USE_L10N:
                    # Formatea usando la localización de Django
                    formatted_number = formats.number_format(
                        balance,
                        decimal_pos=2,
                        force_grouping=True,
                        use_l10n=True
                    )
                    # Reemplaza el separador de miles por defecto ('\xa0') por un punto ('.')
                    formatted_number = formatted_number.replace(NON_BREAKING_SPACE, DESIRED_THOUSAND_SEPARATOR)

                    formatted_balance = f"{formatted_number} €"
                else:
                    # El código de fallback manual ya usa punto como separador de miles
                    base_formatted = f"{balance:.2f}"
                    parts = base_formatted.split('.')
                    integer_part = parts[0]
                    decimal_part = parts[1] if len(parts) > 1 else "00"
                    integer_part_with_sep = ""
                    n = len(integer_part)
                    for i, digit in enumerate(integer_part):
                        integer_part_with_sep += digit
                        if (n - 1 - i) % 3 == 0 and i != n - 1:
                            # Asegura que el fallback también use punto
                            integer_part_with_sep += DESIRED_THOUSAND_SEPARATOR
                    formatted_balance = f"{integer_part_with_sep},{decimal_part} €"

            except (TypeError, ValueError):
                 try:
                     formatted_balance = f"{balance:.2f}".replace('.', ',') + " €"
                 except:
                     formatted_balance = f"{balance} €"


            html += f'<div class="list-group-item">'
            html += f'<div class="row">'
            html += f'<div class="col-md-6">'
            if account.children.exists():
                html += f'<span class="caret" onclick="toggleNested(this)"></span> '
            html += f'<a href="/accounts/edit_account/{ account.id }/">{account.name}</a></div>'
            html += f'<div class="col-md-6">{formatted_balance}</div>'
            html += f'</div>'
            if account.children.exists():
                html += f'<div class="nested" style="display:none;">'
                html += generate_tree_html(account.children.all())
                html += f'</div>'
            html += f'</div>'
        return mark_safe(html)

    return generate_tree_html(accounts)
