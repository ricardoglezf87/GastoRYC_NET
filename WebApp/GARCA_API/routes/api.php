<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\CategoriesTypesController;
use App\Http\Controllers\CategoriesController;
use App\Http\Controllers\AccountsController;
use App\Http\Controllers\TransactionsStatusController;
use App\Http\Controllers\TagsController;
use App\Http\Controllers\PersonsController;
use App\Http\Controllers\InvestmentProductsTypesController;
use App\Http\Controllers\DateCalendarController;
use App\Http\Controllers\InvestmentProductsController;
use App\Http\Controllers\TransactionsController;
use App\Http\Controllers\SplitsController;
use App\Http\Controllers\PeriodsRemindersController;
use App\Http\Controllers\InvestmentProductsPricesController;
use App\Http\Controllers\TransactionsRemindersController;
use App\Http\Controllers\SplitsRemindersController;
use App\Http\Controllers\ExpirationsRemindersController;


/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider and all of them will
| be assigned to the "api" middleware group. Make something great!
|
*/

Route::apiResource('categories_types', CategoriesTypesController::class);
Route::apiResource('categories', CategoriesController::class);
Route::apiResource('accounts',AccountsController::class);
Route::apiResource('transactionsstatus',TransactionsStatusController::class);
Route::apiResource('tags',TagsController::class);
Route::apiResource('persons',PersonsController::class);
Route::apiResource('investmentproductstypes',InvestmentProductsTypesController::class);
Route::apiResource('datecalendar',DateCalendarController::class);
Route::apiResource('investmentproducts',InvestmentProductsController::class);
Route::apiResource('transactions',TransactionsController::class);
Route::apiResource('splits',SplitsController::class);
Route::apiResource('periodsreminders',PeriodsRemindersController::class);
Route::apiResource('investmentproductsprices',InvestmentProductsPricesController::class);
Route::apiResource('transactionsreminders',TransactionsRemindersController::class);
Route::apiResource('splitsreminders',SplitsRemindersController::class);
Route::apiResource('expirationsreminders',ExpirationsRemindersController::class);
