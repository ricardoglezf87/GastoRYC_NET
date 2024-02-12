<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\AccountsTypes;

class AccountsTypesControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/accounts_types');
        $response->assertStatus(200)
                 ->assertJson(AccountsTypes::all()->toArray());
    }

    public function testStore()
    {
        $nuevoAccountsTypes = AccountsTypes::factory()->make()->toArray();

        $response = $this->post('/api/accounts_types', $nuevoAccountsTypes);
        $response->assertStatus(201)
            ->assertJson($nuevoAccountsTypes);
    }

    public function testUpdate()
    {
        $account = AccountsTypes::factory()->create();

        $campo = 'description';
        $valor = 'Nuevo Valor';

        $response = $this->put("/api/accounts_types/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['description' => $valor]);
    }

    public function testDestroy()
    {
        $account = AccountsTypes::factory()->create();

        $response = $this->delete("/api/accounts_types/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'AccountsTypes borrado con éxito']);
        $this->assertNull(AccountsTypes::find($account->id));
    }
}
