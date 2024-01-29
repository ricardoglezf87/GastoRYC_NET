<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class InvestmentProducts extends Model
{
    protected $fillable = ['description', 'investmentProductsTypesId', 'symbol', 'url', 'active'];
}
