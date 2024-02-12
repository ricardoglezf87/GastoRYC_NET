<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@yield('title', 'Mi Aplicación')</title>
    <meta name="csrf-token" content="{{ csrf_token() }}">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <!-- Otros enlaces a hojas de estilo, scripts, etc. van aquí -->
</head>
<body>

    <header>
        <!-- Barra de navegación, encabezado, etc. van aquí -->
    </header>

    <div>
        <main>
            @yield('content')
        </main>
    </div>

    <footer>
        <!-- Pie de página va aquí -->
    </footer>

    <!-- Otros scripts van aquí -->

</body>
</html>
