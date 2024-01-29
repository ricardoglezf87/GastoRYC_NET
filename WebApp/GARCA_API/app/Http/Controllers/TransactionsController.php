<?php

namespace App\Http\Controllers;

use App\Models\Transactions;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class TransactionsController extends Controller
{
    public function index()
    {
        return Transactions::all();
    }

    public function update(Request $request, $id)
    {
        $transactions = Transactions::findOrFail($id);
        // Actualizar el campo específico
        $transactions->{$request->input('campo')} = $request->input('valor');
        $transactions->save();

        return $transactions;
    }

    public function store(Request $request)
    {
        $nuevoTransactions = Transactions::create($request->all());
        return response()->json($nuevoTransactions, 201);
    }

    public function destroy(Request $request, $id)
    {
        $transactions = Transactions::findOrFail($id);
        $transactions->delete();
        return response()->json(['message' => 'Transactions borrado con éxito']);
    }
}
