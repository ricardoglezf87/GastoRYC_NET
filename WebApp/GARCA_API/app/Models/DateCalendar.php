<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class DateCalendar extends Model
{
    protected $fillable = ['date', 'day', 'month', 'year'];
}
