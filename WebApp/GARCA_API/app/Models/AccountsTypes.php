<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class AccountsTypes extends Model
{
    protected $table = 'accounts_types';

    protected $fillable = [
        'description',
    ];
}
