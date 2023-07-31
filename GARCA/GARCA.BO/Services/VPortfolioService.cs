using GARCA.BO.Models;
using GARCA.Utils.IOC;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GARCA.BO.Services
{
    public class VPortfolioService
    {
        public async Task<HashSet<VPortfolio?>?> GetAllAsync()
        {
            HashSet<VPortfolio?> listPortFolio = new();
            foreach (var investmentProducts in
                await DependencyConfig.InvestmentProductsService.GetAllOpened())
            {

                VPortfolio portfolio = new();
                portfolio.Id = investmentProducts.Id;
                portfolio.Description = investmentProducts.Description;
                portfolio.InvestmentProductsTypesid = investmentProducts.InvestmentProductsTypesid;
                portfolio.InvestmentProductsTypes = investmentProducts.InvestmentProductsTypes;
                portfolio.Symbol = investmentProducts.Symbol;
                portfolio.NumShares = await GetNumShares(investmentProducts);

                var lBuy = await GetBuyOperations(investmentProducts);
                var lSell = await GetSellOperations(investmentProducts);

                foreach (var sell in lSell)
                {
                    var shares = sell.NumShares;
                    if (shares is not null and > 0)
                    {
                        foreach (var buy in lBuy)
                        {
                            if (buy.NumShares is not null and not 0)
                            {
                                if (shares >= -buy.NumShares)
                                {
                                    shares += buy.NumShares;
                                    buy.NumShares = 0;
                                }
                                else
                                {
                                    buy.NumShares += shares;
                                    shares = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
                portfolio.CostShares = lBuy?.Sum(x => x.PricesShares * -x.NumShares);
                portfolio.Date = DependencyConfig.InvestmentProductsPricesService.GetLastValueDate(investmentProducts);
                portfolio.Prices = DependencyConfig.InvestmentProductsPricesService.GetActualPrice(investmentProducts);

                listPortFolio.Add(portfolio);
            }
            return listPortFolio;
        }

        private async Task<decimal?> GetNumShares(InvestmentProducts? investmentProducts)
        {
            return await Task.Run(() => DependencyConfig.TransactionsService.GetByInvestmentProduct(investmentProducts)?.Sum(x => -x.NumShares));
        }

        private async Task<HashSet<Transactions?>?> GetBuyOperations(InvestmentProducts? investmentProducts)
        {
            return await Task.Run(() => DependencyConfig.TransactionsService.GetByInvestmentProduct(investmentProducts)?.Where(x => x.NumShares < 0).OrderBy(x => x.Date).ToHashSet());
        }

        private async Task<HashSet<Transactions?>?> GetSellOperations(InvestmentProducts? investmentProducts)
        {
            return await Task.Run(() => DependencyConfig.TransactionsService.GetByInvestmentProduct(investmentProducts)?.Where(x => x.NumShares > 0).OrderBy(x => x.Date).ToHashSet());
        }
    }
}
