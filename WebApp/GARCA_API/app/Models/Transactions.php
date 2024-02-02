<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class Transactions extends Model
{
    use HasFactory;
    
    protected $fillable = ['date', 'accountsId', 'personsId', 'tagsId', 'categoriesId', 'amountIn', 'amountOut', 'tranferId'];
}
