<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\PeriodsReminders;

class PeriodsRemindersControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/periods_reminders');
        $response->assertStatus(200)
                 ->assertJson(PeriodsReminders::all()->toArray());
    }

    public function testStore()
    {
        $nuevoPeriodsReminders = PeriodsReminders::factory()->make()->toArray();

        $response = $this->post('/api/periods_reminders', $nuevoPeriodsReminders);
        $response->assertStatus(201)
            ->assertJson($nuevoPeriodsReminders);
    }

    public function testUpdate()
    {
        $account = PeriodsReminders::factory()->create();

        $campo = 'description';
        $valor = 'Nuevo Valor';

        $response = $this->put("/api/periods_reminders/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['description' => $valor]);
    }

    public function testDestroy()
    {
        $account = PeriodsReminders::factory()->create();

        $response = $this->delete("/api/periods_reminders/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'PeriodsReminders borrado con éxito']);
        $this->assertNull(PeriodsReminders::find($account->id));
    }
}
