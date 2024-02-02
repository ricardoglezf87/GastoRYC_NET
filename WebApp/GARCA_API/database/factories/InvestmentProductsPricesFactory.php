<?php

namespace Database\Factories;

use App\Models\InvestmentProductsPrices;
use Illuminate\Database\Eloquent\Factories\Factory;

class InvestmentProductsPricesFactory extends Factory
{
    protected $model = InvestmentProductsPrices::class;

    public function definition()
    {
        return [
            'date' => $this->faker->date,
            'investmentProductsid' => $this->faker->numberBetween(1, 10), // Puedes ajustar esto según tus necesidades
            'prices' => $this->faker->randomFloat(2, 10, 1000), // Ejemplo de precio aleatorio con 2 decimales
        ];
    }
}
