<?php

namespace Database\Factories;

use App\Models\TransactionsReminders;
use Illuminate\Database\Eloquent\Factories\Factory;

class TransactionsRemindersFactory extends Factory
{
    protected $model = TransactionsReminders::class;

    public function definition()
    {
        return [
            'periodsRemindersId' => $this->faker->numberBetween(1, 10),
            'autoRegister' => $this->faker->boolean,
            'date' => $this->faker->date,
            'accountId' => $this->faker->numberBetween(1, 20),
            'personId' => $this->faker->numberBetween(1, 30),
            'tagId' => $this->faker->numberBetween(1, 40),
            'categoryId' => $this->faker->numberBetween(1, 50),
            'amountIn' => $this->faker->randomFloat(2, 10, 100),
            'amountOut' => $this->faker->randomFloat(2, 10, 100),
            'memo' => $this->faker->sentence,
            'transactionStatusId' => $this->faker->numberBetween(1, 5),
        ];
    }
}
