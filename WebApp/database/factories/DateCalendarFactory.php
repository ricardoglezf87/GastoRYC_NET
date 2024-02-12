<?php

namespace Database\Factories;

use App\Models\DateCalendar;
use Illuminate\Database\Eloquent\Factories\Factory;

class DateCalendarFactory extends Factory
{
    protected $model = DateCalendar::class;

    public function definition()
    {
        return [
            'date'  => $this->faker->date,
            'day'   => $this->faker->dayOfWeek,
            'month' => $this->faker->month,
            'year'  => $this->faker->year,
        ];
    }
}
