<?php

namespace App\Filament\Resources\SplitsResource\Pages;

use App\Filament\Resources\SplitsResource;
use Filament\Actions;
use Filament\Resources\Pages\EditRecord;

class EditSplits extends EditRecord
{
    protected static string $resource = SplitsResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\ViewAction::make(),
            Actions\DeleteAction::make(),
        ];
    }
}
