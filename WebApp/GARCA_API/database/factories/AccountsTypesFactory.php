<?php

namespace Database\Factories;

use App\Models\AccountsTypes;
use Illuminate\Database\Eloquent\Factories\Factory;

class AccountsTypesFactory extends Factory
{
    protected $model = AccountsTypes::class;

    public function definition()
    {
        return [
            'description' => $this->faker->word, 
        ];
    }
}
