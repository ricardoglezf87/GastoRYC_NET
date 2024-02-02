<?php

namespace Database\Factories;

use App\Models\CategoriesTypes;
use Illuminate\Database\Eloquent\Factories\Factory;

class CategoriesTypesFactory extends Factory
{
    protected $model = CategoriesTypes::class;

    public function definition()
    {
        return [
            'description' => $this->faker->word,
        ];
    }
}
