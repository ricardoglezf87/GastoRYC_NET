using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAOLib.Migrations
{
    /// <inheritdoc />
    public partial class AddSymbolInvestmentProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "symbol",
                table: "investmentProducts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "symbol",
                table: "investmentProducts");
        }
    }
}
