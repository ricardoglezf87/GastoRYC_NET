<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class InvestmentProductsPrices extends Model
{
    use HasFactory;

    protected $table = 'investment_products_prices';

    protected $fillable = ['date', 'investmentProductsid', 'prices'];
}
