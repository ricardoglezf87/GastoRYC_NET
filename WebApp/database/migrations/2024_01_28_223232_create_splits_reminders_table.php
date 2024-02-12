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
        Schema::create('splits_reminders', function (Blueprint $table) {
            $table->id();
            $table->integer('transactionsId')->nullable();
            $table->integer('tagsId')->nullable();
            $table->integer('categoryId')->nullable();
            $table->decimal('amountIn', 10, 2)->nullable();
            $table->decimal('amountOut', 10, 2)->nullable();
            $table->string('memo')->nullable();
            $table->integer('tranferId')->nullable();
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('splits_reminders');
    }
};
