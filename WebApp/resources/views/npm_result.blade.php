<!-- resources/views/npm_result.blade.php -->

@extends('layouts.app')

@section('content')
    <h1>Resultado del Comando npm</h1>
    <div>
        {!! $output !!}
    </div>
@endsection
