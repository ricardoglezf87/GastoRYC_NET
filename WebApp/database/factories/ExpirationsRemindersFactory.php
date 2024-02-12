<?php

namespace Database\Factories;

use App\Models\ExpirationsReminders;
use Illuminate\Database\Eloquent\Factories\Factory;

class ExpirationsRemindersFactory extends Factory
{
    protected $model = ExpirationsReminders::class;

    public function definition()
    {
        return [
            'date' => $this->faker->date,
            'transactionsRemindersid' => $this->faker->numberBetween(1, 100), 
            'done' => $this->faker->boolean,
        ];
    }
}
