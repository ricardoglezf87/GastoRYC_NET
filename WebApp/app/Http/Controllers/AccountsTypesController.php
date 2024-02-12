<?php
// En el archivo TareaEstadoController.php

namespace App\Http\Controllers;
use Illuminate\Http\Request;

class AccountsTypesController extends Controller
{
    public function index()
    {
        return view('accounts_types.index');
    }
}
