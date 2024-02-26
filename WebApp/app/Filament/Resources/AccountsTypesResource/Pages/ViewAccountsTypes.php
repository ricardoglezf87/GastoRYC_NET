<?php

namespace App\Filament\Resources\AccountsTypesResource\Pages;

use App\Filament\Resources\AccountsTypesResource;
use Filament\Actions;
use Filament\Resources\Pages\ViewRecord;

class ViewAccountsTypes extends ViewRecord
{
    protected static string $resource = AccountsTypesResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\EditAction::make(),
        ];
    }
}
