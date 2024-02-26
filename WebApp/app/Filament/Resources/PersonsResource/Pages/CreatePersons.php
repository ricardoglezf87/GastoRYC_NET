<?php

namespace App\Filament\Resources\PersonsResource\Pages;

use App\Filament\Resources\PersonsResource;
use Filament\Actions;
use Filament\Resources\Pages\CreateRecord;

class CreatePersons extends CreateRecord
{
    protected static string $resource = PersonsResource::class;
}
