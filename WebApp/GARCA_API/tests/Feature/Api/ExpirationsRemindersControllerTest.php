<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\ExpirationsReminders;

class ExpirationsRemindersControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/expirations_reminders');
        $response->assertStatus(200)
                 ->assertJson(ExpirationsReminders::all()->toArray());
    }

    public function testStore()
    {
        $nuevoExpirationsReminders = ExpirationsReminders::factory()->make()->toArray();

        $response = $this->post('/api/expirations_reminders', $nuevoExpirationsReminders);
        $response->assertStatus(201)
            ->assertJson($nuevoExpirationsReminders);
    }

    public function testUpdate()
    {
        $account = ExpirationsReminders::factory()->create();

        $campo = 'transactionsRemindersid';
        $valor = random_int(1, 30);

        $response = $this->put("/api/expirations_reminders/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['transactionsRemindersid' => $valor]);
    }

    public function testDestroy()
    {
        $account = ExpirationsReminders::factory()->create();

        $response = $this->delete("/api/expirations_reminders/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'ExpirationsReminders borrado con éxito']);
        $this->assertNull(ExpirationsReminders::find($account->id));
    }
}
