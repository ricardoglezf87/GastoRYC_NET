using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDDLib.Migrations
{
    /// <inheritdoc />
    public partial class PersonLastCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "categoryid",
                table: "persons",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_persons_categoryid",
                table: "persons",
                column: "categoryid");

            migrationBuilder.AddForeignKey(
                name: "FK_persons_categories_categoryid",
                table: "persons",
                column: "categoryid",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.Sql(@"
                update persons 
                set
	                categoryid = ( SELECT categoryid 
				                  FROM (
				                    SELECT categoryid, COUNT(categoryid) AS category_count
				                    FROM transactions
				                    where personid = persons.id
				                    GROUP BY personid,categoryid
				                  )
				                  order by category_count desc
				                  limit 1
					             );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_persons_categories_categoryid",
                table: "persons");

            migrationBuilder.DropIndex(
                name: "IX_persons_categoryid",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "categoryid",
                table: "persons");
        }
    }
}
