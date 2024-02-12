<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\Splits;

class SplitsControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/splits');
        $response->assertStatus(200)
                 ->assertJson(Splits::all()->toArray());
    }

    public function testStore()
    {
        $nuevoSplits = Splits::factory()->make()->toArray();

        $response = $this->post('/api/splits', $nuevoSplits);
        $response->assertStatus(201)
            ->assertJson($nuevoSplits);
    }

    public function testUpdate()
    {
        $account = Splits::factory()->create();

        $campo = 'categoryId';
        $valor = random_int(1, 30);

        $response = $this->put("/api/splits/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['categoryId' => $valor]);
    }

    public function testDestroy()
    {
        $account = Splits::factory()->create();

        $response = $this->delete("/api/splits/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'Splits borrado con éxito']);
        $this->assertNull(Splits::find($account->id));
    }
}
