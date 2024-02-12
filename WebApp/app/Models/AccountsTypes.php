<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class AccountsTypes extends Model
{
    use HasFactory;

    protected $table = 'accounts_types';

    protected $fillable = [
        'description',
    ];
}
