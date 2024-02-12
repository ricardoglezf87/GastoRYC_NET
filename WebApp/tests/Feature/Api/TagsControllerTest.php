<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\Tags;

class TagsControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/tags');
        $response->assertStatus(200)
                 ->assertJson(Tags::all()->toArray());
    }

    public function testStore()
    {
        $nuevoTags = Tags::factory()->make()->toArray();

        $response = $this->post('/api/tags', $nuevoTags);
        $response->assertStatus(201)
            ->assertJson($nuevoTags);
    }

    public function testUpdate()
    {
        $account = Tags::factory()->create();

        $campo = 'description';
        $valor = 'Nuevo Valor';

        $response = $this->put("/api/tags/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['description' => $valor]);
    }

    public function testDestroy()
    {
        $account = Tags::factory()->create();

        $response = $this->delete("/api/tags/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'Tags borrado con éxito']);
        $this->assertNull(Tags::find($account->id));
    }
}
