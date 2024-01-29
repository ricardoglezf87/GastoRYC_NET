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
        Schema::create('transactions_reminders', function (Blueprint $table) {
            $table->id();
            $table->integer('periodsRemindersId');
            $table->boolean('autoRegister')->nullable();
            $table->date('date')->nullable();
            $table->integer('accountsId')->nullable();
            $table->integer('personsId')->nullable();
            $table->integer('tagsId')->nullable();
            $table->integer('categoriesId')->nullable();
            $table->decimal('amountIn', 10, 2)->nullable();
            $table->decimal('amountOut', 10, 2)->nullable();
            $table->string('memo')->nullable();
            $table->integer('transactionsStatusId')->nullable();
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('transactions_reminders');
    }
};
