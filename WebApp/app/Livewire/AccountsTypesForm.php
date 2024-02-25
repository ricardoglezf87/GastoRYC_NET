<?php

namespace App\Livewire;

use Livewire\Component;
use Livewire\Attributes\On;
use App\Models\AccountsTypes;

class AccountsTypesForm extends Component
{
    public $id;
    public $description;
    

    public function mount()
    {
        $this->resetValues();
    }

    public function resetValues()
    {
        $this->reset(['id', 'description']);
    }

    #[On('setID')]
    public function setID($id)
    {
        $this->id = $id;
    }

    public function saveObject()
    {
        AccountsTypes::create([
            'id' => $this->id,
            'description' => $this->description,
        ]);
        $this->resetValues();
        $this->dispatch('object-added');
    }

    public function cancelObject()
    {
        $this->resetValues();
        $this->dispatch('object-added');
    }

    public function render()
    {
        if ($this->id) {
           $obj = AccountsTypes::find($this->id);

           if ($obj) {
                $this->id = $obj->id;
                $this->description = $obj->description;
           }
       } else {
           $this->resetValues();
       }

        return view('accounts_types.form_component');
    }
}
