<?php

namespace App\Http\Controllers\Api;

use App\Models\InvestmentProducts;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class InvestmentProductsController extends Controller
{
    public function index()
    {
        return InvestmentProducts::all();
    }

    public function update(Request $request, $id)
    {
        $investmentproducts = InvestmentProducts::findOrFail($id);
        // Actualizar el campo específico
        $investmentproducts->{$request->input('campo')} = $request->input('valor');
        $investmentproducts->save();

        return $investmentproducts;
    }

    public function store(Request $request)
    {
        $nuevoInvestmentProducts = InvestmentProducts::create($request->all());
        return response()->json($nuevoInvestmentProducts, 201);
    }

    public function destroy(Request $request, $id)
    {
        $investmentproducts = InvestmentProducts::findOrFail($id);
        $investmentproducts->delete();
        return response()->json(['message' => 'InvestmentProducts borrado con éxito']);
    }
}
