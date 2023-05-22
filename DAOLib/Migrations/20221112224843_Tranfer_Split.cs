using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAOLib.Migrations
{
    public partial class Tranfer_Split : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "transactionid",
                table: "transactions",
                newName: "tranferid");

            migrationBuilder.AddColumn<int>(
                name: "tranferSplitid",
                table: "transactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tranferid",
                table: "splits",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tranferSplitid",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "tranferid",
                table: "splits");

            migrationBuilder.RenameColumn(
                name: "tranferid",
                table: "transactions",
                newName: "transactionid");
        }
    }
}
