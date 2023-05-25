using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace DAOLib.Migrations
{
    /// <inheritdoc />
    public partial class addExpiracionReminders : Migration
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
                    transactionsRemindersid = table.Column<int>(type: "INTEGER", nullable: true),
                    done = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expirationsReminders", x => x.id);
                    table.ForeignKey(
                        name: "FK_expirationsReminders_transactionsReminders_transactionsRemindersid",
                        column: x => x.transactionsRemindersid,
                        principalTable: "transactionsReminders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_expirationsReminders_transactionsRemindersid",
                table: "expirationsReminders",
                column: "transactionsRemindersid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expirationsReminders");
        }
    }
}
