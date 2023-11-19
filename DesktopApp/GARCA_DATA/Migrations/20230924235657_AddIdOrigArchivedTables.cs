//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace GARCA.DAO.Migrations
//{
//    /// <inheritdoc />
//    public partial class AddIdOrigArchivedTables : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.AddColumn<int>(
//                name: "idOriginal",
//                table: "TransactionsArchived",
//                type: "INTEGER",
//                nullable: true);

//            migrationBuilder.AddColumn<int>(
//                name: "idOriginal",
//                table: "SplitsArchived",
//                type: "INTEGER",
//                nullable: true);

//            migrationBuilder.Sql("INSERT INTO categories(id,description,categoriesTypesid) VALUES (-2,'Regulación Cierre',4);");
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropColumn(
//                name: "idOriginal",
//                table: "TransactionsArchived");

//            migrationBuilder.DropColumn(
//                name: "idOriginal",
//                table: "SplitsArchived");

//            migrationBuilder.Sql("delete from categories where id = -2;");
//        }
//    }
//}
