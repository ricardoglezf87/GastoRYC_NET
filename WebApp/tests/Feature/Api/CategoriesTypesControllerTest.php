<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\CategoriesTypes;

class CategoriesTypesControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/categories_types');
        $response->assertStatus(200)
                 ->assertJson(CategoriesTypes::all()->toArray());
    }

    public function testStore()
    {
        $nuevoCategoriesTypes = CategoriesTypes::factory()->make()->toArray();

        $response = $this->post('/api/categories_types', $nuevoCategoriesTypes);
        $response->assertStatus(201)
            ->assertJson($nuevoCategoriesTypes);
    }

    public function testUpdate()
    {
        $account = CategoriesTypes::factory()->create();

        $campo = 'description';
        $valor = 'Nuevo Valor';

        $response = $this->put("/api/categories_types/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['description' => $valor]);
    }

    public function testDestroy()
    {
        $account = CategoriesTypes::factory()->create();

        $response = $this->delete("/api/categories_types/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'CategoriesTypes borrado con éxito']);
        $this->assertNull(CategoriesTypes::find($account->id));
    }
}
