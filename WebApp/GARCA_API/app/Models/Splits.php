<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Splits extends Model
{
    protected $fillable = ['transactionsId', 'tagsId', 'categoriesId', 'amountIn', 'amountOut', 'memo', 'tranferId'];
}
