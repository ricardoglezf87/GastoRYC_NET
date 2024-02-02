<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\Accounts;

class AccountsControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/accounts');
        $response->assertStatus(200)
                 ->assertJson(Accounts::all()->toArray());
    }

    public function testStore()
    {
        $nuevoAccounts = Accounts::factory()->make()->toArray();

        $response = $this->post('/api/accounts', $nuevoAccounts);
        $response->assertStatus(201)
            ->assertJson($nuevoAccounts);
    }

    public function testUpdate()
    {
        $account = Accounts::factory()->create();

        $campo = 'description';
        $valor = 'Nuevo Valor';

        $response = $this->put("/api/accounts/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['description' => $valor]);
    }

    public function testDestroy()
    {
        $account = Accounts::factory()->create();

        $response = $this->delete("/api/accounts/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'Accounts borrado con éxito']);
        $this->assertNull(Accounts::find($account->id));
    }
}
