<?php

namespace App\Livewire;

use Livewire\Component;
use App\Models\AccountsTypes;

class AccountsTypesGrid extends Component
{
    public $accounts_types;
    public $nuevaLinea;
    public $headers;

    public function mount()
    {
        $this->loadAccountType();
        $this->nuevaLinea = [
            'description' => '',
        ];
        $this->headers = [
            ['key' => 'id', 'label' => '#', 'class' => 'text-red-400'], # <-- css
            ['key' => 'description', 'label' => 'Descripción'],
        ];
    }

    public function updateAccountType($id, $campo, $valor)
    {
        $accountType = AccountsTypes::findOrFail($id);
        $accountType->$campo = $valor;
        $accountType->save();
    }

    public function createAccountType()
    {
        $this->validate([
            'nuevaLinea.description' => 'required',
        ]);

        AccountsTypes::create($this->nuevaLinea);

        $this->nuevaLinea = [
            'description' => '',
        ];

        $this->loadAccountType();
    }

    public function deleteAccountType($id)
    {
       // if (confirm("¿Estás seguro de que quieres borrar este account_type?")) {
            AccountsTypes::findOrFail($id)->delete();
            $this->loadAccountType();
       // }
    }

    public function render()
    {
        return view('accounts_types.AccountsTypesGrid');
    }

    private function loadAccountType()
    {
        $this->accounts_types = AccountsTypes::all();
    }
}
