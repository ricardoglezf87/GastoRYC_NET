<?php

namespace App\Filament\Resources\AccountsTypesResource\Pages;

use App\Filament\Resources\AccountsTypesResource;
use Filament\Actions;
use Filament\Resources\Pages\EditRecord;

class EditAccountsTypes extends EditRecord
{
    protected static string $resource = AccountsTypesResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\ViewAction::make(),
            Actions\DeleteAction::make(),
        ];
    }
}
