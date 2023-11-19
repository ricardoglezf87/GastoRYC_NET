//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace GARCA.DAO.Migrations
//{
//    /// <inheritdoc />
//    public partial class removeTransferReminder : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropColumn(
//                name: "tranferSplitid",
//                table: "TransactionsReminders");

//            migrationBuilder.DropColumn(
//                name: "tranferid",
//                table: "TransactionsReminders");
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.AddColumn<int>(
//                name: "tranferSplitid",
//                table: "TransactionsReminders",
//                type: "INTEGER",
//                nullable: true);

//            migrationBuilder.AddColumn<int>(
//                name: "tranferid",
//                table: "TransactionsReminders",
//                type: "INTEGER",
//                nullable: true);
//        }
//    }
//}
