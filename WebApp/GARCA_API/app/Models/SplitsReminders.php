<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class SplitsReminders extends Model
{
    use HasFactory;

    protected $table = 'splits_reminders';

    protected $fillable = ['transactionsId', 'tagsId', 'categoriesId', 'amountIn', 'amountOut', 'memo', 'tranferId'];
}
