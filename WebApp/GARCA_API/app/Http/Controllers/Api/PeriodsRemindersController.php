<?php

namespace App\Http\Controllers\Api;

use App\Models\PeriodsReminders;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class PeriodsRemindersController extends Controller
{
    public function index()
    {
        return PeriodsReminders::all();
    }

    public function update(Request $request, $id)
    {
        $periodsreminders = PeriodsReminders::findOrFail($id);
        // Actualizar el campo específico
        $periodsreminders->{$request->input('campo')} = $request->input('valor');
        $periodsreminders->save();

        return $periodsreminders;
    }

    public function store(Request $request)
    {
        $nuevoPeriodsReminders = PeriodsReminders::create($request->all());
        return response()->json($nuevoPeriodsReminders, 201);
    }

    public function destroy(Request $request, $id)
    {
        $periodsreminders = PeriodsReminders::findOrFail($id);
        $periodsreminders->delete();
        return response()->json(['message' => 'PeriodsReminders borrado con éxito']);
    }
}
