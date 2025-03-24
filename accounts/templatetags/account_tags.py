from django import template
from django.utils.safestring import mark_safe

register = template.Library()

@register.simple_tag
def generate_account_tree(accounts):
    def generate_tree_html(accounts):
        html = ''
        for account in accounts:
            html += f'<div class="list-group-item">'
            html += f'<div class="row">'
            html += f'<div class="col-md-6">'
            if account.children.exists():
                html += f'<span class="caret" onclick="toggleNested(this)"></span> '
            html += f'<a href="/accounts/edit_account/{ account.id }">{account.name}</a></div>'
            html += f'<div class="col-md-6">{account.get_balance()}</div>'
            html += f'</div>'
            if account.children.exists():
                html += f'<div class="nested" style="display:none;">'
                html += generate_tree_html(account.children.all())
                html += f'</div>'
            html += f'</div>'
        return mark_safe(html)

    return generate_tree_html(accounts)
