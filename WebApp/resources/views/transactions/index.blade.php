@extends('layouts.app')



@section('content')

    <h1>Movimientos</h1>

    <div id="transactions"></div>

    @vite(['resources/css/transactions/styles.css', 'resources/js/transactions/scripts.js'])

@endsection
