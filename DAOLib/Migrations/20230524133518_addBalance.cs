using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAOLib.Migrations
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
