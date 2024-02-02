<?php

namespace Database\Factories;

use App\Models\TransactionsStatus;
use Illuminate\Database\Eloquent\Factories\Factory;

class TransactionsStatusFactory extends Factory
{
    protected $model = TransactionsStatus::class;

    public function definition()
    {
        return [
            'description' => $this->faker->word, 
        ];
    }
}

