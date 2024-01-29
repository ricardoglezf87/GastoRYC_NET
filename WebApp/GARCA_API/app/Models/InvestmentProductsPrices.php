<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class InvestmentProductsPrices extends Model
{
    protected $fillable = ['date', 'investmentProductsid', 'prices'];
}
