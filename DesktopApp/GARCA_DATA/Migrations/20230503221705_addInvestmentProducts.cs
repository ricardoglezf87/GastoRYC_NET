using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GARCA.DAO.Migrations
{
    /// <inheritdoc />
    public partial class addInvestmentProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "investmentProducts",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investmentProducts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "investmentProductsPrices",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    investmentProductsid = table.Column<int>(type: "INTEGER", nullable: true),
                    prices = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investmentProductsPrices", x => x.id);
                    table.ForeignKey(
                        name: "FK_investmentProductsPrices_investmentProducts_investmentProductsid",
                        column: x => x.investmentProductsid,
                        principalTable: "investmentProducts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_investmentProductsPrices_investmentProductsid",
                table: "investmentProductsPrices",
                column: "investmentProductsid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "investmentProductsPrices");

            migrationBuilder.DropTable(
                name: "investmentProducts");
        }
    }
}
