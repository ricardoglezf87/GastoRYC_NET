<?php

namespace App\Filament\Resources\PersonsResource\Pages;

use App\Filament\Resources\PersonsResource;
use Filament\Actions;
use Filament\Resources\Pages\ViewRecord;

class ViewPersons extends ViewRecord
{
    protected static string $resource = PersonsResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\EditAction::make(),
        ];
    }
}
