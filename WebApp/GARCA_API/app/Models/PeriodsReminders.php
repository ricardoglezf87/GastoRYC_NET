<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class PeriodsReminders extends Model
{
    use HasFactory;

    protected $table = 'periods_reminders';

    protected $fillable = ['description'];
}
