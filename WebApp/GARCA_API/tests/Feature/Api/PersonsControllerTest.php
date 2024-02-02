<?php

namespace Tests\Feature\Api;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Tests\TestCase;
use App\Models\Persons;

class PersonsControllerTest extends TestCase
{
    use RefreshDatabase;

    public function testIndex()
    {
        $response = $this->get('/api/persons');
        $response->assertStatus(200)
                 ->assertJson(Persons::all()->toArray());
    }

    public function testStore()
    {
        $nuevoPersons = Persons::factory()->make()->toArray();

        $response = $this->post('/api/persons', $nuevoPersons);
        $response->assertStatus(201)
            ->assertJson($nuevoPersons);
    }

    public function testUpdate()
    {
        $account = Persons::factory()->create();

        $campo = 'name';
        $valor = 'Nuevo Valor';

        $response = $this->put("/api/persons/{$account->id}", ['campo' => $campo, 'valor' => $valor]);
        $response->assertStatus(200)
            ->assertJson(['name' => $valor]);
    }

    public function testDestroy()
    {
        $account = Persons::factory()->create();

        $response = $this->delete("/api/persons/{$account->id}");
        $response->assertStatus(200)
            ->assertJson(['message' => 'Persons borrado con éxito']);
        $this->assertNull(Persons::find($account->id));
    }
}
