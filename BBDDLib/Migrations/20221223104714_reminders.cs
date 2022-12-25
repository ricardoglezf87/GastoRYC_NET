using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDDLib.Migrations
{
    /// <inheritdoc />
    public partial class reminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "periodsReminders",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_periodsReminders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transactionsReminders",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    perdiodRemindersid = table.Column<int>(type: "INTEGER", nullable: true),
                    periodsRemindersid = table.Column<int>(type: "INTEGER", nullable: true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    accountid = table.Column<int>(type: "INTEGER", nullable: true),
                    personid = table.Column<int>(type: "INTEGER", nullable: true),
                    tagid = table.Column<int>(type: "INTEGER", nullable: true),
                    categoryid = table.Column<int>(type: "INTEGER", nullable: true),
                    amountIn = table.Column<decimal>(type: "TEXT", nullable: true),
                    amountOut = table.Column<decimal>(type: "TEXT", nullable: true),
                    tranferid = table.Column<int>(type: "INTEGER", nullable: true),
                    tranferSplitid = table.Column<int>(type: "INTEGER", nullable: true),
                    memo = table.Column<string>(type: "TEXT", nullable: true),
                    transactionStatusid = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactionsReminders", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactionsReminders_accounts_accountid",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactionsReminders_categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactionsReminders_periodsReminders_periodsRemindersid",
                        column: x => x.periodsRemindersid,
                        principalTable: "periodsReminders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactionsReminders_persons_personid",
                        column: x => x.personid,
                        principalTable: "persons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactionsReminders_tags_tagid",
                        column: x => x.tagid,
                        principalTable: "tags",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactionsReminders_transactionsStatus_transactionStatusid",
                        column: x => x.transactionStatusid,
                        principalTable: "transactionsStatus",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "splitsReminders",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    transactionid = table.Column<int>(type: "INTEGER", nullable: true),
                    tagid = table.Column<int>(type: "INTEGER", nullable: true),
                    categoryid = table.Column<int>(type: "INTEGER", nullable: true),
                    amountIn = table.Column<decimal>(type: "TEXT", nullable: true),
                    amountOut = table.Column<decimal>(type: "TEXT", nullable: true),
                    memo = table.Column<string>(type: "TEXT", nullable: true),
                    tranferid = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_splitsReminders", x => x.id);
                    table.ForeignKey(
                        name: "FK_splitsReminders_categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_splitsReminders_tags_tagid",
                        column: x => x.tagid,
                        principalTable: "tags",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_splitsReminders_transactionsReminders_transactionid",
                        column: x => x.transactionid,
                        principalTable: "transactionsReminders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_splitsReminders_categoryid",
                table: "splitsReminders",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_splitsReminders_tagid",
                table: "splitsReminders",
                column: "tagid");

            migrationBuilder.CreateIndex(
                name: "IX_splitsReminders_transactionid",
                table: "splitsReminders",
                column: "transactionid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminders_accountid",
                table: "transactionsReminders",
                column: "accountid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminders_categoryid",
                table: "transactionsReminders",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminders_periodsRemindersid",
                table: "transactionsReminders",
                column: "periodsRemindersid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminders_personid",
                table: "transactionsReminders",
                column: "personid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminders_tagid",
                table: "transactionsReminders",
                column: "tagid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminders_transactionStatusid",
                table: "transactionsReminders",
                column: "transactionStatusid");



            migrationBuilder.Sql("INSERT INTO periodsReminders([description]) VALUES"
                 + " ('Diario'),('Semanal'),('Mensual'),('Bimestral'),('Trimestral'),('Semestral'),('Anual'); ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "splitsReminders");

            migrationBuilder.DropTable(
                name: "transactionsReminders");

            migrationBuilder.DropTable(
                name: "periodsReminders");
        }
    }
}
