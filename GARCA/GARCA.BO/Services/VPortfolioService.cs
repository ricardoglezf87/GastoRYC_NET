using GARCA.BO.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GARCA.Utils.IOC;

namespace GARCA.BO.Services
{
    public class VPortfolioService
    {
        public async Task<HashSet<VPortfolio?>?> getAllAsync()
        {
            HashSet<VPortfolio?>? listPortFolio = new();
            foreach (var investmentProducts in
                await DependencyConfig.iInvestmentProductsService.getAllOpened())
            {

                VPortfolio portfolio = new();
                portfolio.id = investmentProducts.id;
                portfolio.description = investmentProducts.description;
                portfolio.investmentProductsTypesid = investmentProducts.investmentProductsTypesid;
                portfolio.investmentProductsTypes = investmentProducts.investmentProductsTypes;
                portfolio.symbol = investmentProducts.symbol;
                portfolio.numShares = await getNumShares(investmentProducts);

                var lBuy = await getBuyOperations(investmentProducts);
                var lSell = await getSellOperations(investmentProducts);

                foreach (var sell in lSell)
                {
                    var shares = sell.numShares;
                    if (shares is not null and > 0)
                    {
                        foreach (var buy in lBuy)
                        {
                            if (buy.numShares is not null and not 0)
                            {
                                if (shares >= -buy.numShares)
                                {
                                    shares += buy.numShares;
                                    buy.numShares = 0;
                                }
                                else
                                {
                                    buy.numShares += shares;
                                    shares = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
                portfolio.costShares = lBuy?.Sum(x => x.pricesShares * -x.numShares);
                portfolio.date = DependencyConfig.iInvestmentProductsPricesService.getLastValueDate(investmentProducts);
                portfolio.prices = DependencyConfig.iInvestmentProductsPricesService.getActualPrice(investmentProducts);

                listPortFolio.Add(portfolio);
            }
            return listPortFolio;
        }

        private async Task<decimal?> getNumShares(InvestmentProducts? investmentProducts)
        {
            return await Task.Run(() => DependencyConfig.iTransactionsService.getByInvestmentProduct(investmentProducts)?.Sum(x => -x.numShares));
        }

        private async Task<HashSet<Transactions?>?> getBuyOperations(InvestmentProducts? investmentProducts)
        {
            return await Task.Run(() => DependencyConfig.iTransactionsService.getByInvestmentProduct(investmentProducts)?.Where(x => x.numShares < 0).OrderBy(x => x.date).ToHashSet());
        }

        private async Task<HashSet<Transactions?>?> getSellOperations(InvestmentProducts? investmentProducts)
        {
            return await Task.Run(() => DependencyConfig.iTransactionsService.getByInvestmentProduct(investmentProducts)?.Where(x => x.numShares > 0).OrderBy(x => x.date).ToHashSet());
        }
    }
}
