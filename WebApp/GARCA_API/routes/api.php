<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\Api\TagsController;
use App\Http\Controllers\Api\SplitsController;
use App\Http\Controllers\Api\PersonsController;
use App\Http\Controllers\Api\AccountsController;
use App\Http\Controllers\Api\CategoriesController;
use App\Http\Controllers\Api\DateCalendarController;
use App\Http\Controllers\Api\TransactionsController;
use App\Http\Controllers\Api\AccountsTypesController;
use App\Http\Controllers\Api\CategoriesTypesController;
use App\Http\Controllers\Api\SplitsRemindersController;
use App\Http\Controllers\Api\PeriodsRemindersController;
use App\Http\Controllers\Api\InvestmentProductsController;
use App\Http\Controllers\Api\TransactionsStatusController;
use App\Http\Controllers\Api\ExpirationsRemindersController;
use App\Http\Controllers\Api\TransactionsRemindersController;
use App\Http\Controllers\Api\InvestmentProductsTypesController;
use App\Http\Controllers\Api\InvestmentProductsPricesController;


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
Route::apiResource('accounts_types',AccountsTypesController::class);
Route::apiResource('accounts',AccountsController::class);
Route::apiResource('transactions_status',TransactionsStatusController::class);
Route::apiResource('tags',TagsController::class);
Route::apiResource('persons',PersonsController::class);
Route::apiResource('investment_products_types',InvestmentProductsTypesController::class);
Route::apiResource('investment_products_prices',InvestmentProductsPricesController::class);
Route::apiResource('investment_products',InvestmentProductsController::class);
Route::apiResource('date_calendar',DateCalendarController::class);
Route::apiResource('transactions',TransactionsController::class);
Route::apiResource('splits',SplitsController::class);
Route::apiResource('periods_reminders',PeriodsRemindersController::class);
Route::apiResource('transactions_reminders',TransactionsRemindersController::class);
Route::apiResource('splits_reminders',SplitsRemindersController::class);
Route::apiResource('expirations_reminders',ExpirationsRemindersController::class);
