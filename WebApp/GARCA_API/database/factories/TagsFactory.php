<?php

namespace Database\Factories;

use App\Models\Tags;
use Illuminate\Database\Eloquent\Factories\Factory;

class TagsFactory extends Factory
{
    protected $model = Tags::class;

    public function definition()
    {
        return [
            'description' => $this->faker->word, // Puedes ajustar esto según tus necesidades
        ];
    }
}
