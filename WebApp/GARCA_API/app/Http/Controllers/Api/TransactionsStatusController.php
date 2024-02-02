<?php

namespace App\Http\Controllers\Api;

use App\Models\TransactionsStatus;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class TransactionsStatusController extends Controller
{
    public function index()
    {
        return TransactionsStatus::all();
    }

    public function update(Request $request, $id)
    {
        $transactionsstatus = TransactionsStatus::findOrFail($id);
        // Actualizar el campo específico
        $transactionsstatus->{$request->input('campo')} = $request->input('valor');
        $transactionsstatus->save();

        return $transactionsstatus;
    }

    public function store(Request $request)
    {
        $nuevoTransactionsStatus = TransactionsStatus::create($request->all());
        return response()->json($nuevoTransactionsStatus, 201);
    }

    public function destroy(Request $request, $id)
    {
        $transactionsstatus = TransactionsStatus::findOrFail($id);
        $transactionsstatus->delete();
        return response()->json(['message' => 'TransactionsStatus borrado con éxito']);
    }
}
