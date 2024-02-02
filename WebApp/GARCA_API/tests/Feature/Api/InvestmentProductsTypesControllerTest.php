<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\InvestmentProductsTypes;

class InvestmentProductsTypesControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/investment_products_types');
        $response->assertStatus(200)
                 ->assertJson(InvestmentProductsTypes::all()->toArray());
    }

    public function testStore()
    {
        $nuevoInvestmentProductsTypes = InvestmentProductsTypes::factory()->make()->toArray();

        $response = $this->post('/api/investment_products_types', $nuevoInvestmentProductsTypes);
        $response->assertStatus(201)
            ->assertJson($nuevoInvestmentProductsTypes);
    }

    public function testUpdate()
    {
        $account = InvestmentProductsTypes::factory()->create();

        $campo = 'description';
        $valor = 'Nuevo Valor';

        $response = $this->put("/api/investment_products_types/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['description' => $valor]);
    }

    public function testDestroy()
    {
        $account = InvestmentProductsTypes::factory()->create();

        $response = $this->delete("/api/investment_products_types/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'InvestmentProductsTypes borrado con éxito']);
        $this->assertNull(InvestmentProductsTypes::find($account->id));
    }
}
