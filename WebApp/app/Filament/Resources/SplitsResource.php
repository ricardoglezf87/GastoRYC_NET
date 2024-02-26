<?php

namespace App\Filament\Resources;

use App\Filament\Resources\SplitsResource\Pages;
use App\Filament\Resources\SplitsResource\RelationManagers;
use App\Models\Splits;
use Filament\Forms;
use Filament\Forms\Form;
use Filament\Resources\Resource;
use Filament\Tables;
use Filament\Tables\Table;
use Illuminate\Database\Eloquent\Builder;
use Illuminate\Database\Eloquent\SoftDeletingScope;

class SplitsResource extends Resource
{
    protected static ?string $model = Splits::class;

    protected static ?string $navigationIcon = 'heroicon-o-rectangle-stack';

    public static function form(Form $form): Form
    {
        return $form
            ->schema([
                Forms\Components\TextInput::make('transactionsId')
                    ->numeric(),
                Forms\Components\TextInput::make('tagsId')
                    ->numeric(),
                Forms\Components\TextInput::make('categoryId')
                    ->numeric(),
                Forms\Components\TextInput::make('amountIn')
                    ->numeric(),
                Forms\Components\TextInput::make('amountOut')
                    ->numeric(),
                Forms\Components\TextInput::make('memo')
                    ->maxLength(255),
                Forms\Components\TextInput::make('tranferId')
                    ->numeric(),
            ]);
    }

    public static function table(Table $table): Table
    {
        return $table
            ->columns([
                Tables\Columns\TextColumn::make('transactionsId')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('tagsId')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('categoryId')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('amountIn')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('amountOut')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('memo')
                    ->searchable(),
                Tables\Columns\TextColumn::make('tranferId')
                    ->numeric()
                    ->sortable(),
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
            'index' => Pages\ListSplits::route('/'),
            'create' => Pages\CreateSplits::route('/create'),
            'view' => Pages\ViewSplits::route('/{record}'),
            'edit' => Pages\EditSplits::route('/{record}/edit'),
        ];
    }
}
