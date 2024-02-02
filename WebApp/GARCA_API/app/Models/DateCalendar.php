<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class DateCalendar extends Model
{
    use HasFactory;

    protected $table = 'date_calendars';

    protected $fillable = ['date', 'day', 'month', 'year'];
}
