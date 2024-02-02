<?php

namespace Database\Factories;

use App\Models\Splits;
use Illuminate\Database\Eloquent\Factories\Factory;

class SplitsFactory extends Factory
{
    protected $model = Splits::class;

    public function definition()
    {
        return [
            'transactionsId' => $this->faker->numberBetween(1, 100), 
            'tagsId' => $this->faker->numberBetween(1, 10),
            'categoriesId' => $this->faker->numberBetween(1, 20),
            'amountIn' => $this->faker->randomFloat(2, 10, 100),
            'amountOut' => $this->faker->randomFloat(2, 10, 100),
            'memo' => $this->faker->sentence,
            'tranferId' => $this->faker->numberBetween(1, 50),
        ];
    }
}
