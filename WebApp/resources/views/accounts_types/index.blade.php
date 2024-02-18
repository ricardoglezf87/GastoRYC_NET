@extends('layouts.app')
@livewireStyles

@section('content')
    @vite('resources/css/accounts_types/styles.css')
    @livewire('AccountsTypesGrid')
@endsection

@livewireScripts
