<?php

namespace App\Http\Controllers;

use App\Models\CategoriesTypes;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class CategoriesTypesController extends Controller
{
    public function index()
    {
        return CategoriesTypes::all();
    }

    public function update(Request $request, $id)
    {
        $categoriestypes = CategoriesTypes::findOrFail($id);
        // Actualizar el campo específico
        $categoriestypes->{$request->input('campo')} = $request->input('valor');
        $categoriestypes->save();

        return $categoriestypes;
    }

    public function store(Request $request)
    {
        $nuevoCategoriesTypes = CategoriesTypes::create($request->all());
        return response()->json($nuevoCategoriesTypes, 201);
    }

    public function destroy(Request $request, $id)
    {
        $categoriestypes = CategoriesTypes::findOrFail($id);
        $categoriestypes->delete();
        return response()->json(['message' => 'CategoriesTypes borrado con éxito']);
    }
}
