using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GARCA.DAO.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                drop view VResumeStockTransactions;
            ");

            migrationBuilder.Sql(@"
                drop view vLastPrices;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                create view vLastPrices
                as
                    select *
	                FROM investmentProductsPrices ipp 
		                inner join (select investmentProductsid, max(date) date
					                FROM investmentProductsPrices
					                group by investmentProductsid) lDate on lDate.investmentProductsid = ipp.investmentProductsid  
						                and lDate.date = ipp.date 
            ");

            migrationBuilder.Sql(@"
                create view VResumeStockTransactions
                as
                    select investmentProductsid,sum(-numShares) numShares, sum(-numShares*pricesShares) costShares
                    from transactions t 
                    where investmentProductsid is not null
                    group by investmentProductsid 
            ");
        }
    }
}
