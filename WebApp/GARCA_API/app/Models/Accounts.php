<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Accounts extends Model
{
    protected $fillable = ['description', 'accountsTypesId', 'categoryid', 'closed'];
}
