<?php

namespace Database\Factories;

use App\Models\Transactions;
use Illuminate\Database\Eloquent\Factories\Factory;

class TransactionsFactory extends Factory
{
    protected $model = Transactions::class;

    public function definition()
    {
        return [
            'date' => $this->faker->date,
            'accountsId' => $this->faker->numberBetween(1, 10), 
            'personsId' => $this->faker->numberBetween(1, 20),
            'tagsId' => $this->faker->numberBetween(1, 30),
            'categoriesId' => $this->faker->numberBetween(1, 40),
            'amountIn' => $this->faker->randomFloat(2, 10, 100),
            'amountOut' => $this->faker->randomFloat(2, 10, 100),
            'tranferId' => $this->faker->numberBetween(1, 50),
        ];
    }
}
