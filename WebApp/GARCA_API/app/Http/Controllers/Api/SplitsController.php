<?php

namespace App\Http\Controllers\Api;

use App\Models\Splits;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class SplitsController extends Controller
{
    public function index()
    {
        return Splits::all();
    }

    public function update(Request $request, $id)
    {
        $splits = Splits::findOrFail($id);
        // Actualizar el campo específico
        $splits->{$request->input('campo')} = $request->input('valor');
        $splits->save();

        return $splits;
    }

    public function store(Request $request)
    {
        $nuevoSplits = Splits::create($request->all());
        return response()->json($nuevoSplits, 201);
    }

    public function destroy(Request $request, $id)
    {
        $splits = Splits::findOrFail($id);
        $splits->delete();
        return response()->json(['message' => 'Splits borrado con éxito']);
    }
}
