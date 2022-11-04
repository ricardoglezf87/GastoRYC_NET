using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDDLib.Migrations
{
    public partial class AddColTagMemo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "memo",
                table: "transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tagsid",
                table: "transactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_tagsid",
                table: "transactions",
                column: "tagsid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_tags_tagsid",
                table: "transactions",
                column: "tagsid",
                principalTable: "tags",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_tags_tagsid",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropIndex(
                name: "IX_transactions_tagsid",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "memo",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "tagsid",
                table: "transactions");
        }
    }
}
