<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\Categories;

class CategoriesControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/categories');
        $response->assertStatus(200)
                 ->assertJson(Categories::all()->toArray());
    }

    public function testStore()
    {
        $nuevoCategories = Categories::factory()->make()->toArray();

        $response = $this->post('/api/categories', $nuevoCategories);
        $response->assertStatus(201)
            ->assertJson($nuevoCategories);
    }

    public function testUpdate()
    {
        $account = Categories::factory()->create();

        $campo = 'description';
        $valor = 'Nuevo Valor';

        $response = $this->put("/api/categories/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['description' => $valor]);
    }

    public function testDestroy()
    {
        $account = Categories::factory()->create();

        $response = $this->delete("/api/categories/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'Categories borrado con éxito']);
        $this->assertNull(Categories::find($account->id));
    }
}
