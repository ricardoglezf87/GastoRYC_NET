<?php

namespace App\Filament\Resources\TransactionsStatusResource\Pages;

use App\Filament\Resources\TransactionsStatusResource;
use Filament\Actions;
use Filament\Resources\Pages\ListRecords;

class ListTransactionsStatuses extends ListRecords
{
    protected static string $resource = TransactionsStatusResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\CreateAction::make(),
        ];
    }
}
