<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class TransactionsReminders extends Model
{
    use HasFactory;

    protected $table = 'transactions_reminders';

    protected $fillable = ['periodsRemindersId', 'autoRegister', 'date', 'accountsId', 'personsId', 'tagsId', 'categoriesId', 'amountIn', 'amountOut', 'memo', 'transactionsStatusId'];
}
