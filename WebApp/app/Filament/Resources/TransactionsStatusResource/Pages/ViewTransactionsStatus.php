<?php

namespace App\Filament\Resources\TransactionsStatusResource\Pages;

use App\Filament\Resources\TransactionsStatusResource;
use Filament\Actions;
use Filament\Resources\Pages\ViewRecord;

class ViewTransactionsStatus extends ViewRecord
{
    protected static string $resource = TransactionsStatusResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\EditAction::make(),
        ];
    }
}
