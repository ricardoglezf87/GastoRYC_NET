using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDDLib.Migrations
{
    /// <inheritdoc />
    public partial class Vistas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW vBalancebyCategory 
                AS 
               select year,month,categoriesTypesid, categoryid,category,sum(amount) amount
                from (SELECT SUBSTR(date,1,4) year,SUBSTR(date,3,2) month, c.id categoryid, c.description category, t.amountIn-t.amountOut amount,categoriesTypesid
	                FROM transactions t 
	                    inner join categories c ON t.categoryid  = c.id and c.categoriesTypesid in (1,2)
	                union 
	                SELECT SUBSTR(date,1,4) year,SUBSTR(date,3,2) month, c.id categoryid, c.description category, s.amountIn-s.amountOut amount,categoriesTypesid
	                FROM transactions t 
	                    inner join splits s on s.transactionid  = t.id 
	                    inner join categories c ON s.categoryid  = c.id and c.categoriesTypesid in (1,2)
	                ) ut
                group by year,month,categoriesTypesid,categoryid,category;       
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

                drop view vBalancebyCategory;

            ");
        }
    }
}
