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
            'accountId' => $this->faker->numberBetween(1, 10),
            'personId' => $this->faker->numberBetween(1, 20),
            'tagId' => $this->faker->numberBetween(1, 30),
            'categoryId' => $this->faker->numberBetween(1, 40),
            'amountIn' => $this->faker->randomFloat(2, 10, 100),
            'amountOut' => $this->faker->randomFloat(2, 10, 100),
            'tranferId' => $this->faker->numberBetween(1, 50),
            'transactionStatusId' => $this->faker->numberBetween(1, 5),
            'numShares' => $this->faker->randomFloat(6, 10, 100),
            'pricesShares' => $this->faker->randomFloat(6, 10, 100),
            'investmentCategory' => $this->faker->numberBetween(0, 1),
            'balance' => $this->faker->randomFloat(2, 10, 100),
            'orden' => $this->faker->numberBetween(1, 10),
        ];
    }
}
