<?php

namespace App\Http\Controllers\Api;

use App\Models\TransactionsReminders;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class TransactionsRemindersController extends Controller
{
    public function index()
    {
        return TransactionsReminders::all();
    }

    public function update(Request $request, $id)
    {
        $transactionsreminders = TransactionsReminders::findOrFail($id);
        // Actualizar el campo específico
        $transactionsreminders->{$request->input('campo')} = $request->input('valor');
        $transactionsreminders->save();

        return $transactionsreminders;
    }

    public function store(Request $request)
    {
        $nuevoTransactionsReminders = TransactionsReminders::create($request->all());
        return response()->json($nuevoTransactionsReminders, 201);
    }

    public function destroy(Request $request, $id)
    {
        $transactionsreminders = TransactionsReminders::findOrFail($id);
        $transactionsreminders->delete();
        return response()->json(['message' => 'TransactionsReminders borrado con éxito']);
    }
}
