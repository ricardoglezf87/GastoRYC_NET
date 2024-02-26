<?php

namespace App\Filament\Resources\SplitsResource\Pages;

use App\Filament\Resources\SplitsResource;
use Filament\Actions;
use Filament\Resources\Pages\ListRecords;

class ListSplits extends ListRecords
{
    protected static string $resource = SplitsResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\CreateAction::make(),
        ];
    }
}
