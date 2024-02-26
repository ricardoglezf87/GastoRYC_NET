<?php

namespace App\Filament\Resources\TransactionsStatusResource\Pages;

use App\Filament\Resources\TransactionsStatusResource;
use Filament\Actions;
use Filament\Resources\Pages\EditRecord;

class EditTransactionsStatus extends EditRecord
{
    protected static string $resource = TransactionsStatusResource::class;

    protected function getHeaderActions(): array
    {
        return [
            Actions\ViewAction::make(),
            Actions\DeleteAction::make(),
        ];
    }
}
