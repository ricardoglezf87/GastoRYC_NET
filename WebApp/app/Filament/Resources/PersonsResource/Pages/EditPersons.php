<?php

namespace App\Filament\Resources\PersonsResource\Pages;

use App\Filament\Resources\PersonsResource;
use Filament\Actions;
use Filament\Resources\Pages\EditRecord;

class EditPersons extends EditRecord
{
    protected static string $resource = PersonsResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\ViewAction::make(),
            Actions\DeleteAction::make(),
        ];
    }
}
