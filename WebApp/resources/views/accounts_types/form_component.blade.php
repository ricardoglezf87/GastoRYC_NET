<x-mary-modal id="eForm" persistent>

    <x-mary-form wire:submit="saveObject">

        <x-mary-input label="Código" wire:model="id" />
        <x-mary-input label="Descripción" wire:model="description" placeholder="Escribe una descripción" clearable />

        <x-slot:actions>

            <x-mary-button type="submit" label="Guardar" class="btn-primary" />
            <x-mary-button label="Cancelar" wire:click="cancelObject" />

        </x-slot:actions>

    </x-mary-form>

</x-mary-modal>
