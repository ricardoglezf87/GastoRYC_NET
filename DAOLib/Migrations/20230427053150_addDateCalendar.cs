using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace DAOLib.Migrations
{
    /// <inheritdoc />
    public partial class addDateCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                update transactions
                set 
	                date = SUBSTRING(date,7,4) || '-' || SUBSTRING(date,1,2) || '-' || SUBSTRING(date,4,2)
                where LENGTH (date)<=10;
            ");

            migrationBuilder.Sql(@"
                update transactions
                set
	                date = date || ' 00:00:00' 
                where LENGTH (date)<=10;
            ");

            migrationBuilder.CreateTable(
                name: "dateCalendar",
                columns: table => new
                {
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    day = table.Column<int>(type: "INTEGER", nullable: true),
                    month = table.Column<int>(type: "INTEGER", nullable: true),
                    year = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dateCalendar", x => x.date);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dateCalendar");
        }
    }
}
