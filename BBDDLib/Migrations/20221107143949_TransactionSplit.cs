using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDDLib.Migrations
{
    public partial class TransactionSplit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "splits",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    transactionid = table.Column<int>(type: "INTEGER", nullable: true),
                    tagid = table.Column<int>(type: "INTEGER", nullable: true),
                    categoryid = table.Column<int>(type: "INTEGER", nullable: true),
                    amountIn = table.Column<decimal>(type: "TEXT", nullable: true),
                    amountOut = table.Column<decimal>(type: "TEXT", nullable: true),
                    memo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_splits", x => x.id);
                    table.ForeignKey(
                        name: "FK_splits_categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_splits_tags_tagid",
                        column: x => x.tagid,
                        principalTable: "tags",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_splits_transactions_transactionid",
                        column: x => x.transactionid,
                        principalTable: "transactions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_splits_categoryid",
                table: "splits",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_splits_tagid",
                table: "splits",
                column: "tagid");

            migrationBuilder.CreateIndex(
                name: "IX_splits_transactionid",
                table: "splits",
                column: "transactionid");

            migrationBuilder.Sql("INSERT INTO categoriesTypes (description) VALUES ('Especiales');");

            migrationBuilder.Sql("INSERT INTO categories(id,description,categoriesTypesid) VALUES (-1,'Splits...',4);");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "splits");
        }
    }
}
