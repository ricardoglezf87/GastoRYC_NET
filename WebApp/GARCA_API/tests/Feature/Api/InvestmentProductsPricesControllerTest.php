<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\InvestmentProductsPrices;

class InvestmentProductsPricesControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/investment_products_prices');
        $response->assertStatus(200)
                 ->assertJson(InvestmentProductsPrices::all()->toArray());
    }

    public function testStore()
    {
        $nuevoInvestmentProductsPrices = InvestmentProductsPrices::factory()->make()->toArray();

        $response = $this->post('/api/investment_products_prices', $nuevoInvestmentProductsPrices);
        $response->assertStatus(201)
            ->assertJson($nuevoInvestmentProductsPrices);
    }

    public function testUpdate()
    {
        $account = InvestmentProductsPrices::factory()->create();

        $campo = 'prices';
        $valor = random_int(1, 30);

        $response = $this->put("/api/investment_products_prices/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['prices' => $valor]);
    }

    public function testDestroy()
    {
        $account = InvestmentProductsPrices::factory()->create();

        $response = $this->delete("/api/investment_products_prices/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'InvestmentProductsPrices borrado con éxito']);
        $this->assertNull(InvestmentProductsPrices::find($account->id));
    }
}
