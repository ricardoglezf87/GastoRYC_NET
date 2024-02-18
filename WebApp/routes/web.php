<?php

use App\Models\Transactions;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\NpmController;
use App\Http\Controllers\TransactionsController;
use App\Http\Controllers\AccountsTypesController;

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider and all of them will
| be assigned to the "web" middleware group. Make something great!
|
*/

Route::get('/', [AccountsTypesController::class, 'index'])->name('accounts_types.index');
Route::get('/accounts_types', [AccountsTypesController::class, 'index'])->name('accounts_types.index');
Route::get('/transactions', [TransactionsController::class, 'index'])->name('accounts_types.index');


/*
***  Comandos npm ****
*/

Route::get('/npm/build', [NpmController::class, 'build']);
