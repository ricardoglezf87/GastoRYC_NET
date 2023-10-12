using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GARCA.DAO.Migrations
{
    /// <inheritdoc />
    public partial class createArchivedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionsArchived",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    transactionStatusid = table.Column<int>(type: "INTEGER", nullable: true),
                    investmentProductsid = table.Column<int>(type: "INTEGER", nullable: true),
                    numShares = table.Column<decimal>(type: "TEXT", nullable: true),
                    pricesShares = table.Column<decimal>(type: "TEXT", nullable: true),
                    investmentCategory = table.Column<bool>(type: "INTEGER", nullable: true),
                    balance = table.Column<decimal>(type: "TEXT", nullable: true),
                    orden = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionsArchived", x => x.id);
                    table.ForeignKey(
                        name: "FK_TransactionsArchived_Accounts_accountid",
                        column: x => x.accountid,
                        principalTable: "Accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_TransactionsArchived_Categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "Categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_TransactionsArchived_InvestmentProducts_investmentProductsid",
                        column: x => x.investmentProductsid,
                        principalTable: "InvestmentProducts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_TransactionsArchived_Persons_personid",
                        column: x => x.personid,
                        principalTable: "Persons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_TransactionsArchived_Tags_tagid",
                        column: x => x.tagid,
                        principalTable: "Tags",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_TransactionsArchived_TransactionsStatus_transactionStatusid",
                        column: x => x.transactionStatusid,
                        principalTable: "TransactionsStatus",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SplitsArchived",
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
                    table.PrimaryKey("PK_SplitsArchived", x => x.id);
                    table.ForeignKey(
                        name: "FK_SplitsArchived_Categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "Categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_SplitsArchived_Tags_tagid",
                        column: x => x.tagid,
                        principalTable: "Tags",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_SplitsArchived_TransactionsArchived_transactionid",
                        column: x => x.transactionid,
                        principalTable: "TransactionsArchived",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SplitsArchived_categoryid",
                table: "SplitsArchived",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_SplitsArchived_tagid",
                table: "SplitsArchived",
                column: "tagid");

            migrationBuilder.CreateIndex(
                name: "IX_SplitsArchived_transactionid",
                table: "SplitsArchived",
                column: "transactionid");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsArchived_accountid",
                table: "TransactionsArchived",
                column: "accountid");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsArchived_categoryid",
                table: "TransactionsArchived",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsArchived_investmentProductsid",
                table: "TransactionsArchived",
                column: "investmentProductsid");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsArchived_personid",
                table: "TransactionsArchived",
                column: "personid");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsArchived_tagid",
                table: "TransactionsArchived",
                column: "tagid");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsArchived_transactionStatusid",
                table: "TransactionsArchived",
                column: "transactionStatusid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SplitsArchived");

            migrationBuilder.DropTable(
                name: "TransactionsArchived");
        }
    }
}
