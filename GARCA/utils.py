from urllib.parse import urlparse


def add_breadcrumb(request, text, url, max_length=10):
    breadcrumbs = request.session.get('breadcrumbs', [])
    path_without_query = urlparse(url).path
    new_breadcrumb = (text, path_without_query)
    breadcrumb_set = {f"{text}-{url}" for text, url in breadcrumbs}
    new_breadcrumb_str = f"{new_breadcrumb[0]}-{new_breadcrumb[1]}"

    if new_breadcrumb_str not in breadcrumb_set:
        breadcrumbs.append(new_breadcrumb)

    # Find the index of the new breadcrumb
    new_index = -1
    for i, (t, u) in enumerate(breadcrumbs):
        if f"{t}-{u}" == new_breadcrumb_str:
            new_index = i
            break

    if new_index != -1:
        # Remove breadcrumbs after the new index
        breadcrumbs = breadcrumbs[:new_index + 1]

    # Trim breadcrumbs if the length exceeds max_length
    if len(breadcrumbs) > max_length:
        breadcrumbs = breadcrumbs[:max_length]

    request.session['breadcrumbs'] = breadcrumbs

def clear_breadcrumbs(request):
    request.session['breadcrumbs'] = []

def remove_breadcrumb(request, text, url):
    breadcrumbs = request.session.get('breadcrumbs', [])
    path_without_query = urlparse(url).path
    breadcrumb_to_remove = (text, path_without_query)
    breadcrumb_set = {f"{text}-{url}" for text, url in breadcrumbs}
    breadcrumb_to_remove_str = f"{breadcrumb_to_remove[0]}-{breadcrumb_to_remove[1]}"

    new_breadcrumbs = [bc for bc in breadcrumbs if f"{bc[0]}-{bc[1]}" != breadcrumb_to_remove_str]
    request.session['breadcrumbs'] = new_breadcrumbs

def go_back_breadcrumb(request):
    breadcrumbs = request.session.get('breadcrumbs', [])
    if len(breadcrumbs) > 1:
        breadcrumbs.pop()  # Remove the last breadcrumb
    request.session['breadcrumbs'] = breadcrumbs

def get_breadcrumbs(request):
    return request.session.get('breadcrumbs', [])