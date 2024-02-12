<?php

namespace App\Http\Controllers\Api;

use App\Models\ExpirationsReminders;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class ExpirationsRemindersController extends Controller
{
    public function index()
    {
        return ExpirationsReminders::all();
    }

    public function update(Request $request, $id)
    {
        $expirationsreminders = ExpirationsReminders::findOrFail($id);
        // Actualizar el campo específico
        $expirationsreminders->{$request->input('campo')} = $request->input('valor');
        $expirationsreminders->save();

        return $expirationsreminders;
    }

    public function store(Request $request)
    {
        $nuevoExpirationsReminders = ExpirationsReminders::create($request->all());
        return response()->json($nuevoExpirationsReminders, 201);
    }

    public function destroy(Request $request, $id)
    {
        $expirationsreminders = ExpirationsReminders::findOrFail($id);
        $expirationsreminders->delete();
        return response()->json(['message' => 'ExpirationsReminders borrado con éxito']);
    }
}
