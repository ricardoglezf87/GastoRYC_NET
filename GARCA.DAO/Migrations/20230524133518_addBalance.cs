using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GARCA.DAO.Migrations
{
    /// <inheritdoc />
    public partial class addBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.AddColumn<decimal>(
                name: "balance",
                table: "Transactions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.DropColumn(
                name: "balance",
                table: "Transactions");
        }
    }
}
