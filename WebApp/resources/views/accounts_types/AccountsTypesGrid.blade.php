<div>


    <x-table
        :headers=$headers
        :rows=$accounts_types
        striped
        wire:model="accounts_types"
        @row-click="console.log($event.detail)"
        />



    <table class="accounts_types-table">
        <thead>
            <tr>
                <th>Descripción</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>
                    <input type="text" wire:model="nuevaLinea.description">
                </td>
                <td>
                    <button wire:click="createAccountType"> <i class="fas fa-plus"></i> Nuevo estado</button>
                </td>
            </tr>
            @foreach ($accounts_types as $account_type)
                <tr key="{{ $account_type->id }}">
                    <td>
                        <input type="text" value="{{ $account_type->description }}"
                            wire:keyup.enter="updateAccountType({{ $account_type->id }}, 'description', $event.target.value)"
                            wire:blur="updateAccountType({{ $account_type->id }}, 'description', $event.target.value)">
                    </td>
                    <td>
                        <button wire:click="deleteAccountType({{ $account_type->id }})" class="delete-btn">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                </tr>
            @endforeach
        </tbody>
    </table>
</div>
