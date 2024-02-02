<?php

namespace App\Http\Controllers\Api;

use App\Models\SplitsReminders;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class SplitsRemindersController extends Controller
{
    public function index()
    {
        return SplitsReminders::all();
    }

    public function update(Request $request, $id)
    {
        $splitsreminders = SplitsReminders::findOrFail($id);
        // Actualizar el campo específico
        $splitsreminders->{$request->input('campo')} = $request->input('valor');
        $splitsreminders->save();

        return $splitsreminders;
    }

    public function store(Request $request)
    {
        $nuevoSplitsReminders = SplitsReminders::create($request->all());
        return response()->json($nuevoSplitsReminders, 201);
    }

    public function destroy(Request $request, $id)
    {
        $splitsreminders = SplitsReminders::findOrFail($id);
        $splitsreminders->delete();
        return response()->json(['message' => 'SplitsReminders borrado con éxito']);
    }
}
