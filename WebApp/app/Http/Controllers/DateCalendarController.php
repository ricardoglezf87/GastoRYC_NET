<?php

namespace App\Http\Controllers;

use App\Models\DateCalendar;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;

class DateCalendarController extends Controller
{
    public function index()
    {
        return DateCalendar::all();
    }

    public function update(Request $request, $id)
    {
        $datecalendar = DateCalendar::findOrFail($id);
        // Actualizar el campo específico
        $datecalendar->{$request->input('campo')} = $request->input('valor');
        $datecalendar->save();

        return $datecalendar;
    }

    public function store(Request $request)
    {
        $nuevoDateCalendar = DateCalendar::create($request->all());
        return response()->json($nuevoDateCalendar, 201);
    }

    public function destroy(Request $request, $id)
    {
        $datecalendar = DateCalendar::findOrFail($id);
        $datecalendar->delete();
        return response()->json(['message' => 'DateCalendar borrado con éxito']);
    }
}
