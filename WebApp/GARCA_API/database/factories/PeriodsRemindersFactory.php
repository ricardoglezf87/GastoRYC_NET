<?php

namespace Database\Factories;

use App\Models\PeriodsReminders;
use Illuminate\Database\Eloquent\Factories\Factory;

class PeriodsRemindersFactory extends Factory
{
    protected $model = PeriodsReminders::class;

    public function definition()
    {
        return [
            'description' => $this->faker->word, 
        ];
    }
}
