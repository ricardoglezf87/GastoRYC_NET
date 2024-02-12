<?php

namespace App\Http\Controllers;

use App\Models\Tags;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class TagsController extends Controller
{
    public function index()
    {
        return Tags::all();
    }

    public function update(Request $request, $id)
    {
        $tags = Tags::findOrFail($id);
        // Actualizar el campo específico
        $tags->{$request->input('campo')} = $request->input('valor');
        $tags->save();

        return $tags;
    }

    public function store(Request $request)
    {
        $nuevoTags = Tags::create($request->all());
        return response()->json($nuevoTags, 201);
    }

    public function destroy(Request $request, $id)
    {
        $tags = Tags::findOrFail($id);
        $tags->delete();
        return response()->json(['message' => 'Tags borrado con éxito']);
    }
}
