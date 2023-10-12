using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GARCA.DAO.Migrations
{
    /// <inheritdoc />
    public partial class InvestmentProductsTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"

                drop view vBalancebyCategory;

            ");

            migrationBuilder.AddColumn<int>(
                name: "investmentProductsid",
                table: "transactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_investmentProductsid",
                table: "transactions",
                column: "investmentProductsid");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_investmentProducts_investmentProductsid",
                table: "transactions",
                column: "investmentProductsid",
                principalTable: "investmentProducts",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_investmentProducts_investmentProductsid",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_investmentProductsid",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "investmentProductsid",
                table: "transactions");
        }
    }
}
