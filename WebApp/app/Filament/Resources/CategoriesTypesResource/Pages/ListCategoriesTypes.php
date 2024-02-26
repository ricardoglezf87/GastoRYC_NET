<?php

namespace App\Filament\Resources\CategoriesTypesResource\Pages;

use App\Filament\Resources\CategoriesTypesResource;
use Filament\Actions;
use Filament\Resources\Pages\ListRecords;

class ListCategoriesTypes extends ListRecords
{
    protected static string $resource = CategoriesTypesResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\CreateAction::make(),
        ];
    }
}
