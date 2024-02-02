<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\SplitsReminders;

class SplitsRemindersControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/splits_reminders');
        $response->assertStatus(200)
                 ->assertJson(SplitsReminders::all()->toArray());
    }

    public function testStore()
    {
        $nuevoSplitsReminders = SplitsReminders::factory()->make()->toArray();

        $response = $this->post('/api/splits_reminders', $nuevoSplitsReminders);
        $response->assertStatus(201)
            ->assertJson($nuevoSplitsReminders);
    }

    public function testUpdate()
    {
        $account = SplitsReminders::factory()->create();

        $campo = 'categoriesId';
        $valor = random_int(1, 30);

        $response = $this->put("/api/splits_reminders/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['categoriesId' => $valor]);
    }

    public function testDestroy()
    {
        $account = SplitsReminders::factory()->create();

        $response = $this->delete("/api/splits_reminders/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'SplitsReminders borrado con éxito']);
        $this->assertNull(SplitsReminders::find($account->id));
    }
}
