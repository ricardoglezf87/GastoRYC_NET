@extends('layouts.app')


@section('content')
    @vite('resources/css/accounts_types/styles.css')

    {{-- @livewire('AccountsTypesForm') --}}

    @livewire('AccountsTypesGrid')

    {{-- <x-mary-button label="Abrir Modal" class="btn-primary" onclick="eForm.showModal()" /> --}}
@endsection
