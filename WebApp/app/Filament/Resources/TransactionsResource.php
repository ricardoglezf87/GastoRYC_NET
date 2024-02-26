<?php

namespace App\Filament\Resources;

use App\Filament\Resources\TransactionsResource\Pages;
use App\Filament\Resources\TransactionsResource\RelationManagers;
use App\Models\Transactions;
use Filament\Forms;
use Filament\Forms\Form;
use Filament\Resources\Resource;
use Filament\Tables;
use Filament\Tables\Table;
use Illuminate\Database\Eloquent\Builder;
use Illuminate\Database\Eloquent\SoftDeletingScope;

class TransactionsResource extends Resource
{
    protected static ?string $model = Transactions::class;

    protected static ?string $navigationIcon = 'heroicon-o-rectangle-stack';

    public static function form(Form $form): Form
    {
        return $form
            ->schema([
                Forms\Components\DatePicker::make('date')
                    ->required(),
                Forms\Components\TextInput::make('accountid')
                    ->required()
                    ->numeric(),
                Forms\Components\TextInput::make('personid')
                    ->numeric(),
                Forms\Components\TextInput::make('tagid')
                    ->numeric(),
                Forms\Components\TextInput::make('categoryid')
                    ->required()
                    ->numeric(),
                Forms\Components\TextInput::make('amountin')
                    ->numeric(),
                Forms\Components\TextInput::make('amountout')
                    ->numeric(),
                Forms\Components\TextInput::make('tranferid')
                    ->numeric(),
                Forms\Components\TextInput::make('tranfersplitid')
                    ->numeric(),
                Forms\Components\TextInput::make('memo')
                    ->maxLength(255),
                Forms\Components\TextInput::make('transactionStatusId')
                    ->numeric(),
                Forms\Components\TextInput::make('investmentProductid')
                    ->numeric(),
                Forms\Components\TextInput::make('numShares')
                    ->numeric(),
                Forms\Components\TextInput::make('pricesShares')
                    ->numeric(),
                Forms\Components\Toggle::make('investmentCategory'),
                Forms\Components\TextInput::make('balance')
                    ->numeric(),
                Forms\Components\TextInput::make('orden')
                    ->numeric(),
            ]);
    }

    public static function table(Table $table): Table
    {
        return $table
            ->columns([
                Tables\Columns\TextColumn::make('date')
                    ->date()
                    ->sortable(),
                Tables\Columns\TextColumn::make('accountid')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('personid')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('tagid')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('categoryid')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('amountin')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('amountout')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('tranferid')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('tranfersplitid')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('memo')
                    ->searchable(),
                Tables\Columns\TextColumn::make('transactionStatusId')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('investmentProductid')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('numShares')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('pricesShares')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\IconColumn::make('investmentCategory')
                    ->boolean(),
                Tables\Columns\TextColumn::make('balance')
                    ->numeric()
                    ->sortable(),
                Tables\Columns\TextColumn::make('orden')
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
            'index' => Pages\ListTransactions::route('/'),
            'create' => Pages\CreateTransactions::route('/create'),
            'view' => Pages\ViewTransactions::route('/{record}'),
            'edit' => Pages\EditTransactions::route('/{record}/edit'),
        ];
    }
}
