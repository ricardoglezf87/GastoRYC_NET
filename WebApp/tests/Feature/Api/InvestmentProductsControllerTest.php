<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\InvestmentProducts;

class InvestmentProductsControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/investment_products');
        $response->assertStatus(200)
                 ->assertJson(InvestmentProducts::all()->toArray());
    }

    public function testStore()
    {
        $nuevoInvestmentProducts = InvestmentProducts::factory()->make()->toArray();

        $response = $this->post('/api/investment_products', $nuevoInvestmentProducts);
        $response->assertStatus(201)
            ->assertJson($nuevoInvestmentProducts);
    }

    public function testUpdate()
    {
        $account = InvestmentProducts::factory()->create();

        $campo = 'description';
        $valor = 'Nuevo Valor';

        $response = $this->put("/api/investment_products/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['description' => $valor]);
    }

    public function testDestroy()
    {
        $account = InvestmentProducts::factory()->create();

        $response = $this->delete("/api/investment_products/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'InvestmentProducts borrado con éxito']);
        $this->assertNull(InvestmentProducts::find($account->id));
    }
}
