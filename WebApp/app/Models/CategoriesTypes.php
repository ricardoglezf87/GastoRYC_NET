<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class CategoriesTypes extends Model
{
    use HasFactory;

    protected $table = 'categories_types';

    protected $fillable = [
        'description',
    ];

    public function categories()
    {
        return $this->hasMany(Categories::class, 'categoriestypesid', 'id');
    }
}
