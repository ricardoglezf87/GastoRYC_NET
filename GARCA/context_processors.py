def breadcrumbs(request):
    return {'breadcrumbs': request.session.get('breadcrumbs', [])}