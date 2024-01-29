<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class ExpirationsReminders extends Model
{
    protected $fillable = ['date', 'transactionsRemindersid', 'done'];
}
