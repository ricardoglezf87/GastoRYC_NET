using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDDLib.Migrations
{
    /// <inheritdoc />
    public partial class reminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "periodsReminder",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_periodsReminder", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transactionsReminder",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    perdiodReminderid = table.Column<int>(type: "INTEGER", nullable: true),
                    periodsReminderid = table.Column<int>(type: "INTEGER", nullable: true),
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
                    table.PrimaryKey("PK_transactionsReminder", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactionsReminder_accounts_accountid",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactionsReminder_categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactionsReminder_periodsReminder_periodsReminderid",
                        column: x => x.periodsReminderid,
                        principalTable: "periodsReminder",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactionsReminder_persons_personid",
                        column: x => x.personid,
                        principalTable: "persons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactionsReminder_tags_tagid",
                        column: x => x.tagid,
                        principalTable: "tags",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactionsReminder_transactionsStatus_transactionStatusid",
                        column: x => x.transactionStatusid,
                        principalTable: "transactionsStatus",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "splitsReminder",
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
                    table.PrimaryKey("PK_splitsReminder", x => x.id);
                    table.ForeignKey(
                        name: "FK_splitsReminder_categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_splitsReminder_tags_tagid",
                        column: x => x.tagid,
                        principalTable: "tags",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_splitsReminder_transactionsReminder_transactionid",
                        column: x => x.transactionid,
                        principalTable: "transactionsReminder",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_splitsReminder_categoryid",
                table: "splitsReminder",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_splitsReminder_tagid",
                table: "splitsReminder",
                column: "tagid");

            migrationBuilder.CreateIndex(
                name: "IX_splitsReminder_transactionid",
                table: "splitsReminder",
                column: "transactionid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminder_accountid",
                table: "transactionsReminder",
                column: "accountid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminder_categoryid",
                table: "transactionsReminder",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminder_periodsReminderid",
                table: "transactionsReminder",
                column: "periodsReminderid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminder_personid",
                table: "transactionsReminder",
                column: "personid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminder_tagid",
                table: "transactionsReminder",
                column: "tagid");

            migrationBuilder.CreateIndex(
                name: "IX_transactionsReminder_transactionStatusid",
                table: "transactionsReminder",
                column: "transactionStatusid");



            migrationBuilder.Sql("INSERT INTO periodsReminder([description]) VALUES"
                 + " ('Diario'),('Semanal'),('Mensual'),('Bimestral'),('Trimestral'),('Semestral'),('Anual'); ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "splitsReminder");

            migrationBuilder.DropTable(
                name: "transactionsReminder");

            migrationBuilder.DropTable(
                name: "periodsReminder");
        }
    }
}
