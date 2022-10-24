using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDDLib.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accountsTypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "nvarchar(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accountsTypes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categoriesTypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "nvarchar(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoriesTypes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "nvarchar(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transactionsStatus",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "nvarchar(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactionsStatus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    categoriesTypesid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_categories_categoriesTypes_categoriesTypesid",
                        column: x => x.categoriesTypesid,
                        principalTable: "categoriesTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    accountsTypesid = table.Column<int>(type: "int", nullable: false),
                    categoryid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_accounts_accountsTypes_accountsTypesid",
                        column: x => x.accountsTypesid,
                        principalTable: "accountsTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_accounts_categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "categories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    accountid = table.Column<int>(type: "int", nullable: true),
                    personid = table.Column<int>(type: "int", nullable: true),
                    categoryid = table.Column<int>(type: "int", nullable: true),
                    amountIn = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    amountOut = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    transactionid = table.Column<int>(type: "int", nullable: true),
                    transactionStatusid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactions_accounts_accountid",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactions_categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactions_persons_personid",
                        column: x => x.personid,
                        principalTable: "persons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactions_transactionsStatus_transactionStatusid",
                        column: x => x.transactionStatusid,
                        principalTable: "transactionsStatus",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_accountsTypesid",
                table: "accounts",
                column: "accountsTypesid");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_categoryid",
                table: "accounts",
                column: "categoryid",
                unique: true,
                filter: "[categoryid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_categories_categoriesTypesid",
                table: "categories",
                column: "categoriesTypesid");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_accountid",
                table: "transactions",
                column: "accountid");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_categoryid",
                table: "transactions",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_personid",
                table: "transactions",
                column: "personid");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_transactionStatusid",
                table: "transactions",
                column: "transactionStatusid");
        }

        

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropTable(
                name: "transactionsStatus");

            migrationBuilder.DropTable(
                name: "accountsTypes");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "categoriesTypes");
        }
    }
}
