<?php

namespace Database\Factories;

use App\Models\Persons;
use Illuminate\Database\Eloquent\Factories\Factory;

class PersonsFactory extends Factory
{
    protected $model = Persons::class;

    public function definition()
    {
        return [
            'name' => $this->faker->name,
            'categoryid' => $this->faker->numberBetween(1, 10), // Puedes ajustar esto según tus necesidades
        ];
    }
}
