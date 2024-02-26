<?php

namespace App\Filament\Resources;

use App\Filament\Resources\AccountsResource\Pages;
use App\Filament\Resources\AccountsResource\RelationManagers;
use App\Models\Accounts;
use Filament\Forms;
use Filament\Forms\Form;
use Filament\Resources\Resource;
use Filament\Tables;
use Filament\Tables\Table;
use Illuminate\Database\Eloquent\Builder;
use Illuminate\Database\Eloquent\SoftDeletingScope;

class AccountsResource extends Resource
{
    protected static ?string $model = Accounts::class;

    protected static ?string $navigationIcon = 'heroicon-o-rectangle-stack';

    public static function form(Form $form): Form
    {
        return $form
            ->schema([
                Forms\Components\Section::make()
                    ->schema([
                        Forms\Components\TextInput::make('description')
                            ->required()
                            ->maxLength(255),
                        Forms\Components\TextInput::make('accountsTypesId')
                            ->required()
                            ->numeric(),
                        Forms\Components\TextInput::make('categoryid')
                            ->numeric(),
                        Forms\Components\Toggle::make('closed')
                            ->required(),
                    ])
            ]);
    }

    public static function table(Table $table): Table
    {
        return $table
            ->columns([
                Tables\Columns\TextColumn::make('description')
                    ->searchable(),
                Tables\Columns\TextColumn::make('accountsTypesId')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('categoryid')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\IconColumn::make('closed')
                    ->boolean(),
                Tables\Columns\TextColumn::make('created_at')
                    ->dateTime()
                    ->sortable()
                    ->toggleable(isToggledHiddenByDefault: true),
                Tables\Columns\TextColumn::make('updated_at')
                    ->dateTime()
                    ->sortable()
                    ->toggleable(isToggledHiddenByDefault: true),
            ])
            ->filters([
                //
            ])
            ->actions([
                Tables\Actions\ViewAction::make(),
                Tables\Actions\EditAction::make(),
            ])
            ->bulkActions([
                Tables\Actions\BulkActionGroup::make([
                    Tables\Actions\DeleteBulkAction::make(),
                ]),
            ]);
    }

    public static function getRelations(): array
    {
        return [
            //
        ];
    }

    public static function getPages(): array
    {
        return [
            'index' => Pages\ListAccounts::route('/'),
            'create' => Pages\CreateAccounts::route('/create'),
            'view' => Pages\ViewAccounts::route('/{record}'),
            'edit' => Pages\EditAccounts::route('/{record}/edit'),
        ];
    }
}
