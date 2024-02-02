<?php

namespace Database\Factories;

use Illuminate\Database\Eloquent\Factories\Factory;
use App\Models\Accounts;


class AccountsFactory extends Factory
{
    protected $model = Accounts::class;

    public function definition(): array
    {
        return [
            'description' => $this->faker->sentence,
            'accountsTypesId' => rand(1, 10),
            'categoryid' => rand(1, 10),
            'closed' => $this->faker->boolean,
        ];
    }
}
