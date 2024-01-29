<?php

namespace App\Http\Controllers;

use App\Models\InvestmentProductsTypes;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class InvestmentProductsTypesController extends Controller
{
    public function index()
    {
        return InvestmentProductsTypes::all();
    }

    public function update(Request $request, $id)
    {
        $investmentproductstypes = InvestmentProductsTypes::findOrFail($id);
        // Actualizar el campo específico
        $investmentproductstypes->{$request->input('campo')} = $request->input('valor');
        $investmentproductstypes->save();

        return $investmentproductstypes;
    }

    public function store(Request $request)
    {
        $nuevoInvestmentProductsTypes = InvestmentProductsTypes::create($request->all());
        return response()->json($nuevoInvestmentProductsTypes, 201);
    }

    public function destroy(Request $request, $id)
    {
        $investmentproductstypes = InvestmentProductsTypes::findOrFail($id);
        $investmentproductstypes->delete();
        return response()->json(['message' => 'InvestmentProductsTypes borrado con éxito']);
    }
}
