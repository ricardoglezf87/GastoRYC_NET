using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAOLib.Migrations
{
    /// <inheritdoc />
    public partial class TransactionInvestment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "numShares",
                table: "transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "pricesShares",
                table: "transactions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "numShares",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "pricesShares",
                table: "transactions");
        }
    }
}
