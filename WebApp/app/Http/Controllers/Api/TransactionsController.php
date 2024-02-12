<?php

namespace App\Http\Controllers\Api;

use App\Models\Transactions;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;
use Symfony\Component\HttpFoundation\StreamedResponse;

class TransactionsController extends Controller
{
    public function index()
    {
        $response = new StreamedResponse(function () {
            $transactions = Transactions::cursor();

            echo '[';

            $first = true;

            foreach ($transactions as $transaction) {
                if (!$first) {
                    echo ',';
                } else {
                    $first = false;
                }

                echo json_encode($transaction->toArray());
            }

            echo ']';
        });

        $response->headers->set('Content-Type', 'application/json');
        $response->headers->set('Cache-Control', 'no-store, no-cache, must-revalidate, max-age=0');
        $response->headers->set('Pragma', 'no-cache');
        $response->headers->set('Expires', 'Fri, 01 Jan 1990 00:00:00 GMT');

        return $response;
        //return Transactions::all();
    }

    public function update(Request $request, $id)
    {
        $transactions = Transactions::findOrFail($id);
        // Actualizar el campo específico
        $transactions->{$request->input('campo')} = $request->input('valor');
        $transactions->save();

        return $transactions;
    }

    public function store(Request $request)
    {
        $nuevoTransactions = Transactions::create($request->all());
        return response()->json($nuevoTransactions, 201);
    }

    public function destroy(Request $request, $id)
    {
        $transactions = Transactions::findOrFail($id);
        $transactions->delete();
        return response()->json(['message' => 'Transactions borrado con éxito']);
    }
}
