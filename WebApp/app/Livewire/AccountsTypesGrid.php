<?php

namespace App\Livewire;

use Livewire\Component;
use Livewire\Attributes\On;
use Filament\Tables\Actions\ActionGroup;
use Filament\Tables\Actions\BulkAction;
use Filament\Tables\Columns\TextColumn;
use Filament\Tables\Concerns\InteractsWithTable;
use Filament\Forms\Concerns\InteractsWithForms;
use Filament\Tables\Contracts\HasTable;
use Filament\Forms\Contracts\HasForms;
use Filament\Tables\Actions\DeleteAction;
use Filament\Tables\Actions\EditAction;
use Filament\Tables\Table;
use Illuminate\Contracts\View\View;
use App\Models\AccountsTypes;

class AccountsTypesGrid extends Component implements HasForms, HasTable
{
    use InteractsWithTable;
    use InteractsWithForms;
    public $accounts_types;

    public function table(Table $table): Table
    {
        return $table
            ->query(AccountsTypes::query())
            ->searchPlaceholder('Buscar por descripción')
            ->actions([
                    EditAction::make(),
                    DeleteAction::make(),
            ])
            ->columns(
                [
                    TextColumn::make('id'),
                    TextColumn::make('description')
                        ->searchable()
                        ->action(function (AccountsTypes $record): void {
                            $this->dispatch('open-post-edit-modal', post: $record->getKey());
                        })
                ]
            );
    }

    public function deleteAccountType($id)
    {
        AccountsTypes::findOrFail($id)->delete();
        $this->loadAccountType();
    }

    #[On('object-added')]
    public function render(): View
    {
        $this->accounts_types = AccountsTypes::all();
        return view('accounts_types.list_component');
    }

    private function loadAccountType()
    {
        $this->accounts_types = AccountsTypes::all();
    }
}
