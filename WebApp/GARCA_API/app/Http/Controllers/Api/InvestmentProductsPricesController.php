<?php

namespace App\Http\Controllers\Api;

use App\Models\InvestmentProductsPrices;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class InvestmentProductsPricesController extends Controller
{
    public function index()
    {
        return InvestmentProductsPrices::all();
    }

    public function update(Request $request, $id)
    {
        $investmentproductsprices = InvestmentProductsPrices::findOrFail($id);
        // Actualizar el campo específico
        $investmentproductsprices->{$request->input('campo')} = $request->input('valor');
        $investmentproductsprices->save();

        return $investmentproductsprices;
    }

    public function store(Request $request)
    {
        $nuevoInvestmentProductsPrices = InvestmentProductsPrices::create($request->all());
        return response()->json($nuevoInvestmentProductsPrices, 201);
    }

    public function destroy(Request $request, $id)
    {
        $investmentproductsprices = InvestmentProductsPrices::findOrFail($id);
        $investmentproductsprices->delete();
        return response()->json(['message' => 'InvestmentProductsPrices borrado con éxito']);
    }
}
