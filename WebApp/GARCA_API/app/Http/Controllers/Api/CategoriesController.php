<?php

namespace App\Http\Controllers\Api;

use App\Models\Categories;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class CategoriesController extends Controller
{
    public function index()
    {
        return Categories::all();
    }

    public function update(Request $request, $id)
    {
        $categories = Categories::findOrFail($id);
        // Actualizar el campo específico
        $categories->{$request->input('campo')} = $request->input('valor');
        $categories->save();

        return $categories;
    }

    public function store(Request $request)
    {
        $nuevoCategories = Categories::create($request->all());
        return response()->json($nuevoCategories, 201);
    }

    public function destroy(Request $request, $id)
    {
        $categories = Categories::findOrFail($id);
        $categories->delete();
        return response()->json(['message' => 'Categories borrado con éxito']);
    }
}
