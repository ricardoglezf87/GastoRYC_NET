<?php

namespace App\Filament\Resources\AccountsTypesResource\Pages;

use App\Filament\Resources\AccountsTypesResource;
use Filament\Actions;
use Filament\Resources\Pages\ListRecords;

class ListAccountsTypes extends ListRecords
{
    protected static string $resource = AccountsTypesResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\CreateAction::make(),
        ];
    }
}
