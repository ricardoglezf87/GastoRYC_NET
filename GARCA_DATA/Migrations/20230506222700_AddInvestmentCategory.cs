//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace GARCA.DAO.Migrations
//{
//    /// <inheritdoc />
//    public partial class AddInvestmentCategory : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.AddColumn<bool>(
//                name: "investmentCategory",
//                table: "transactions",
//                type: "INTEGER",
//                nullable: true);

//            migrationBuilder.Sql(@"
//                update transactions set investmentCategory = true;
//            ");
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropColumn(
//                name: "investmentCategory",
//                table: "transactions");
//        }
//    }
//}
