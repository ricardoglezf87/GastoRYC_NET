<?php

namespace App\Http\Controllers\Api;

use App\Models\Accounts;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class AccountsController extends Controller
{
    public function index()
    {
        return Accounts::all();
    }

    public function update(Request $request, $id)
    {
        $accounts = Accounts::findOrFail($id);
        // Actualizar el campo específico
        $accounts->{$request->input('campo')} = $request->input('valor');
        $accounts->save();

        return $accounts;
    }

    public function store(Request $request)
    {
        $nuevoAccounts = Accounts::create($request->all());
        return response()->json($nuevoAccounts, 201);
    }

    public function destroy(Request $request, $id)
    {
        $accounts = Accounts::findOrFail($id);
        $accounts->delete();
        return response()->json(['message' => 'Accounts borrado con éxito']);
    }
}
