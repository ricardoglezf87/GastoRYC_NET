using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GARCA.DAO.Migrations
{
    /// <inheritdoc />
    public partial class addInvestmentProductsTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "investmentProductsTypesid",
                table: "InvestmentProducts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InvestmentProductsTypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentProductsTypes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentProducts_investmentProductsTypesid",
                table: "InvestmentProducts",
                column: "investmentProductsTypesid");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentProducts_InvestmentProductsTypes_investmentProductsTypesid",
                table: "InvestmentProducts",
                column: "investmentProductsTypesid",
                principalTable: "InvestmentProductsTypes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentProducts_InvestmentProductsTypes_investmentProductsTypesid",
                table: "InvestmentProducts");

            migrationBuilder.DropTable(
                name: "InvestmentProductsTypes");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentProducts_investmentProductsTypesid",
                table: "InvestmentProducts");

            migrationBuilder.DropColumn(
                name: "investmentProductsTypesid",
                table: "InvestmentProducts");
        }
    }
}
