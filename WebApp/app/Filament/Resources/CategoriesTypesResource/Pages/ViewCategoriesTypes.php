<?php

namespace App\Filament\Resources\CategoriesTypesResource\Pages;

use App\Filament\Resources\CategoriesTypesResource;
use Filament\Actions;
use Filament\Resources\Pages\ViewRecord;

class ViewCategoriesTypes extends ViewRecord
{
    protected static string $resource = CategoriesTypesResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\EditAction::make(),
        ];
    }
}
