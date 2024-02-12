<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\TransactionsReminders;

class TransactionsRemindersControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/transactions_reminders');
        $response->assertStatus(200)
                 ->assertJson(TransactionsReminders::all()->toArray());
    }

    public function testStore()
    {
        $nuevoTransactionsReminders = TransactionsReminders::factory()->make()->toArray();

        $response = $this->post('/api/transactions_reminders', $nuevoTransactionsReminders);
        $response->assertStatus(201)
            ->assertJson($nuevoTransactionsReminders);
    }

    public function testUpdate()
    {
        $account = TransactionsReminders::factory()->create();

        $campo = 'categoryId';
        $valor = random_int(1, 30);

        $response = $this->put("/api/transactions_reminders/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['categoryId' => $valor]);
    }

    public function testDestroy()
    {
        $account = TransactionsReminders::factory()->create();

        $response = $this->delete("/api/transactions_reminders/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'TransactionsReminders borrado con éxito']);
        $this->assertNull(TransactionsReminders::find($account->id));
    }
}
