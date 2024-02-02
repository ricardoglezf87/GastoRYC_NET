<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class ExpirationsReminders extends Model
{
    use HasFactory;

    protected $table = 'expirations_reminders';

    protected $fillable = ['date', 'transactionsRemindersid', 'done'];
}
