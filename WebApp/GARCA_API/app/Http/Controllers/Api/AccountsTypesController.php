<?php

namespace App\Http\Controllers\Api;

use App\Models\AccountsTypes;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class AccountsTypesController extends Controller
{
    public function index()
    {
        return AccountsTypes::all();
    }

    public function update(Request $request, $id)
    {
        $accountstypes = AccountsTypes::findOrFail($id);
        // Actualizar el campo específico
        $accountstypes->{$request->input('campo')} = $request->input('valor');
        $accountstypes->save();

        return $accountstypes;
    }

    public function store(Request $request)
    {
        $nuevoAccountsTypes = AccountsTypes::create($request->all());
        return response()->json($nuevoAccountsTypes, 201);
    }

    public function destroy(Request $request, $id)
    {
        $accountstypes = AccountsTypes::findOrFail($id);
        $accountstypes->delete();
        return response()->json(['message' => 'AccountsTypes borrado con éxito']);
    }
}
