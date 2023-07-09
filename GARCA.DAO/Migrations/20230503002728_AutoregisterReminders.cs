using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GARCA.DAO.Migrations
{
    /// <inheritdoc />
    public partial class AutoregisterReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "autoRegister",
                table: "transactionsReminders",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "autoRegister",
                table: "transactionsReminders");
        }
    }
}
