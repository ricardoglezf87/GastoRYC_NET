using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDDLib.Migrations
{
    /// <inheritdoc />
    public partial class ExpiracionReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "expirationsReminders",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    transactaionsRemindersid = table.Column<int>(type: "INTEGER", nullable: true),
                    done = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expirationsReminders", x => x.id);
                    table.ForeignKey(
                        name: "FK_expirationsReminders_transactionsReminders_transactaionsRemindersid",
                        column: x => x.transactaionsRemindersid,
                        principalTable: "transactionsReminders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_expirationsReminders_transactaionsRemindersid",
                table: "expirationsReminders",
                column: "transactaionsRemindersid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expirationsReminders");
        }
    }
}
