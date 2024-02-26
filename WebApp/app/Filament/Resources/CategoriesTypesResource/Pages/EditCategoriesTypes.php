<?php

namespace App\Filament\Resources\CategoriesTypesResource\Pages;

use App\Filament\Resources\CategoriesTypesResource;
use Filament\Actions;
use Filament\Resources\Pages\EditRecord;

class EditCategoriesTypes extends EditRecord
{
    protected static string $resource = CategoriesTypesResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\ViewAction::make(),
            Actions\DeleteAction::make(),
        ];
    }
}
