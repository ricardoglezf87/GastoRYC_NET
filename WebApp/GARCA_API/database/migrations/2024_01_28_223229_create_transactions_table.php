<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('transactions', function (Blueprint $table) {
            $table->id();
            $table->date('date');
            $table->integer('accountsId');
            $table->integer('personsId')->nullable();
            $table->integer('tagsId')->nullable();
            $table->integer('categoriesId');
            $table->decimal('amountIn', 10, 2)->nullable();
            $table->decimal('amountOut', 10, 2)->nullable();
            $table->integer('tranferId')->nullable();
            $table->integer('tranferSplitId')->nullable();
            $table->string('memo')->nullable();
            $table->integer('transactionsStatusId')->nullable();
            $table->integer('investmentProductsId')->nullable();
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('transactions');
    }
};
