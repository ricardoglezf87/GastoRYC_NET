<?php

namespace App\Filament\Resources\SplitsResource\Pages;

use App\Filament\Resources\SplitsResource;
use Filament\Actions;
use Filament\Resources\Pages\ViewRecord;

class ViewSplits extends ViewRecord
{
    protected static string $resource = SplitsResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\EditAction::make(),
        ];
    }
}
