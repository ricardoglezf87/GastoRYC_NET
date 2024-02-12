<?php

namespace App\Http\Controllers\Api;

use App\Models\Persons;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class PersonsController extends Controller
{
    public function index()
    {
        return Persons::all();
    }

    public function update(Request $request, $id)
    {
        $persons = Persons::findOrFail($id);
        // Actualizar el campo específico
        $persons->{$request->input('campo')} = $request->input('valor');
        $persons->save();

        return $persons;
    }

    public function store(Request $request)
    {
        $nuevoPersons = Persons::create($request->all());
        return response()->json($nuevoPersons, 201);
    }

    public function destroy(Request $request, $id)
    {
        $persons = Persons::findOrFail($id);
        $persons->delete();
        return response()->json(['message' => 'Persons borrado con éxito']);
    }
}
