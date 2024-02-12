<?php

namespace Database\Factories;

use App\Models\InvestmentProducts;
use Illuminate\Database\Eloquent\Factories\Factory;

class InvestmentProductsFactory extends Factory
{
    protected $model = InvestmentProducts::class;

    public function definition()
    {
        return [
            'description' => $this->faker->word,
            'investmentProductsTypesId' => $this->faker->numberBetween(1, 10), 
            'symbol' => $this->faker->word,
            'url' => $this->faker->url,
            'active' => $this->faker->boolean,
        ];
    }
}
