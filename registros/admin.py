from django.contrib import admin
from .models import Cuenta, Categoria
from mptt.admin import MPTTModelAdmin

class CuentaAdmin(admin.ModelAdmin):
    list_display = ('nombre', 'saldo')
    search_fields = ('nombre',)

class CategoriaAdmin(MPTTModelAdmin):
    list_display = ('nombre')
    mptt_level_indent = 20

admin.site.register(Cuenta, CuentaAdmin)
admin.site.register(Categoria, CategoriaAdmin)