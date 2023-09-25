using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GARCA.DAO.Migrations
{
    /// <inheritdoc />
    public partial class AddIdOrigArchivedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "idOriginal",
                table: "TransactionsArchived",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "idOriginal",
                table: "SplitsArchived",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idOriginal",
                table: "TransactionsArchived");

            migrationBuilder.DropColumn(
                name: "idOriginal",
                table: "SplitsArchived");
        }
    }
}
