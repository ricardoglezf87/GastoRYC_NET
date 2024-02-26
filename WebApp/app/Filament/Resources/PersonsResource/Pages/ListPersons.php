<?php

namespace App\Filament\Resources\PersonsResource\Pages;

use App\Filament\Resources\PersonsResource;
use Filament\Actions;
use Filament\Resources\Pages\ListRecords;

class ListPersons extends ListRecords
{
    protected static string $resource = PersonsResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\CreateAction::make(),
        ];
    }
}
