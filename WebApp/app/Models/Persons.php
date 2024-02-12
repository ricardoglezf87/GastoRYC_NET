<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class Persons extends Model
{
    use HasFactory;

    protected $fillable = ['name', 'categoryid'];
}
