<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\Transactions;

class TransactionsControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/transactions');
        $response->assertStatus(200)
                 ->assertJson(Transactions::all()->toArray());
    }

    public function testStore()
    {
        $nuevoTransactions = Transactions::factory()->make()->toArray();

        $response = $this->post('/api/transactions', $nuevoTransactions);
        $response->assertStatus(201)
            ->assertJson($nuevoTransactions);
    }

    public function testUpdate()
    {
        $account = Transactions::factory()->create();

        $campo = 'categoryId';
        $valor = random_int(1, 30);

        $response = $this->put("/api/transactions/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['categoryId' => $valor]);
    }

    public function testDestroy()
    {
        $account = Transactions::factory()->create();

        $response = $this->delete("/api/transactions/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'Transactions borrado con éxito']);
        $this->assertNull(Transactions::find($account->id));
    }
}
