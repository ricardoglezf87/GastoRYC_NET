<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class InvestmentProducts extends Model
{
    use HasFactory;

    protected $table = 'investment_products';

    protected $fillable = ['description', 'investmentProductsTypesId', 'symbol', 'url', 'active'];
}
