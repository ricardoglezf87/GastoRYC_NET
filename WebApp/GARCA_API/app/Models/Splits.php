<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class Splits extends Model
{
    use HasFactory;

    protected $fillable = ['transactionsId', 'tagsId', 'categoriesId', 'amountIn', 'amountOut', 'memo', 'tranferId'];
}
