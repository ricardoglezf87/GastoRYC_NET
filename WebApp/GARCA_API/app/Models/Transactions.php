<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Transactions extends Model
{
    protected $fillable = ['date', 'accountsId', 'personsId', 'tagsId', 'categoriesId', 'amountIn', 'amountOut', 'tranferId'];
}
