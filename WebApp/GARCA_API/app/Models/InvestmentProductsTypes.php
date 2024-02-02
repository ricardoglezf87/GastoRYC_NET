<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class InvestmentProductsTypes extends Model
{
    use HasFactory;

    protected $table = 'investment_products_types';

    protected $fillable = ['description'];
}
