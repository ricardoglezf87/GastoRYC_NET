<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\TransactionsStatus;

class TransactionsStatusControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/transactions_status');
        $response->assertStatus(200)
                 ->assertJson(TransactionsStatus::all()->toArray());
    }

    public function testStore()
    {
        $nuevoTransactionsStatus = TransactionsStatus::factory()->make()->toArray();

        $response = $this->post('/api/transactions_status', $nuevoTransactionsStatus);
        $response->assertStatus(201)
            ->assertJson($nuevoTransactionsStatus);
    }

    public function testUpdate()
    {
        $account = TransactionsStatus::factory()->create();

        $campo = 'description';
        $valor = 'Nuevo Valor';

        $response = $this->put("/api/transactions_status/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['description' => $valor]);
    }

    public function testDestroy()
    {
        $account = TransactionsStatus::factory()->create();

        $response = $this->delete("/api/transactions_status/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'TransactionsStatus borrado con éxito']);
        $this->assertNull(TransactionsStatus::find($account->id));
    }
}
