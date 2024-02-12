<?php

namespace Database\Factories;

use App\Models\InvestmentProductsTypes;
use Illuminate\Database\Eloquent\Factories\Factory;

class InvestmentProductsTypesFactory extends Factory
{
    protected $model = InvestmentProductsTypes::class;

    public function definition()
    {
        return [
            'description' => $this->faker->word, 
        ];
    }
}
