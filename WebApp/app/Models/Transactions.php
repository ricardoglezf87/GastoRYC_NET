<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class Transactions extends Model
{
    use HasFactory;

    protected $fillable = ['date', 'accountId', 'personId', 'tagId', 'categoryId', 'amountIn', 'amountOut', 'tranferId','memo','transactionStatusId','investmentProductsid','numShares','pricesShares','investmentCategory','balance','orden'];
}
