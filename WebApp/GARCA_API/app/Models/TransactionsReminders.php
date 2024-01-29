<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class TransactionsReminders extends Model
{
    protected $fillable = ['periodsRemindersId', 'autoRegister', 'date', 'accountsId', 'personsId', 'tagsId', 'categoriesId', 'amountIn', 'amountOut', 'memo', 'transactionsStatusId'];
}
