<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\DateCalendar;

class DateCalendarControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/date_calendar');
        $response->assertStatus(200)
                 ->assertJson(DateCalendar::all()->toArray());
    }

    public function testStore()
    {
        $nuevoDateCalendar = DateCalendar::factory()->make([
            'day' => 1,
        ])->toArray();

        $response = $this->post('/api/date_calendar', $nuevoDateCalendar);
        $response->assertStatus(201)
            ->assertJson($nuevoDateCalendar);
    }

    public function testUpdate()
    {
        $account = DateCalendar::factory()->create([
            'day' => 1,
        ]);

        $campo = 'date';
        $valor = now()->format('Y-m-d');

        $response = $this->put("/api/date_calendar/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['date' => $valor]);
    }

    public function testDestroy()
    {
        $account = DateCalendar::factory()->create([
            'day' => 1,
        ]);

        $response = $this->delete("/api/date_calendar/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'DateCalendar borrado con éxito']);
        $this->assertNull(DateCalendar::find($account->id));
    }
}
