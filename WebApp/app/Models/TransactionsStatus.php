<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class TransactionsStatus extends Model
{
    use HasFactory;

    protected $table = 'transactions_status';

    protected $fillable = ['description'];
}
