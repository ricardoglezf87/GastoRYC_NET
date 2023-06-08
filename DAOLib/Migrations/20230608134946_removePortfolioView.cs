using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAOLib.Migrations
{
    /// <inheritdoc />
    public partial class removePortfolioView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                drop view VPortfolio;
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

            migrationBuilder.Sql(@"
                create view VPortfolio
                as
	                select ip.id, ip.description, ip.symbol, vlp.date, vlp.prices, vrt.numShares, vrt.costShares
                    from investmentProducts ip 
                        left join vLastPrices vlp on ip.id = vlp.investmentProductsid
                        left join VResumeStockTransactions vrt on vrt.investmentProductsid = ip.id
                    where active = 1
            ");
        }
    }
}
