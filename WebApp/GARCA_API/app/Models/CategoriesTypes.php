<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class CategoriesTypes extends Model
{
    protected $table = 'categories_types';

    protected $fillable = [
        'description',
    ];
}
