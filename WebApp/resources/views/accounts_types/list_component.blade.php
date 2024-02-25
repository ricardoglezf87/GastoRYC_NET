<div>

    {{ $this->table }}

    {{-- <x-mary-table :headers=$headers :rows=$accounts_types striped wire:model="accounts_types"
        @row-click="showModal($event)">
        @scope('actions', $accounts_type)
            <x-mary-button icon="o-trash" wire:click="deleteAccountType({{ $accounts_type->id }})" spinner class="btn-sm"
                wire:confirm="¿Estás seguro de que quieres borrar este tipo de cuenta?" />
        @endscope
    </x-mary-table>

    <script>
        function showModal(event) {
            Livewire.dispatch('setID',['id',event.detail.id]);
            eForm.showModal();
        }
    </script> --}}

</div>
