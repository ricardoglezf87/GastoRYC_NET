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
            $table->integer('accountid');
            $table->integer('personid')->nullable();
            $table->integer('tagid')->nullable();
            $table->integer('categoryid');
            $table->decimal('amountin', 10, 2)->nullable();
            $table->decimal('amountout', 10, 2)->nullable();
            $table->integer('tranferid')->nullable();
            $table->integer('tranfersplitid')->nullable();
            $table->string('memo')->nullable();
            $table->integer('transactionStatusId')->nullable();
            $table->integer('investmentProductid')->nullable();
            $table->decimal('numShares', 18, 6)->nullable();
            $table->decimal('pricesShares', 18, 6)->nullable();
            $table->boolean('investmentCategory')->nullable();
            $table->decimal('balance', 10, 2)->nullable();
            $table->integer('orden')->nullable();
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
