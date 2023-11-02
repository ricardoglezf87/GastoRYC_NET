using GARCA.Models;
using GARCA.Utils.Extensions;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
{
    public class VPortfolioService
    {
        public async Task<HashSet<VPortfolio?>?> GetAllAsync()
        {
            HashSet<VPortfolio?> listPortFolio = new();
            foreach (var investmentProducts in
                await iInvestmentProductsService.GetAllOpened())
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
                                    break;
                                }
                            }
                        }
                    }
                }
                portfolio.CostShares = lBuy?.Sum(x => x.PricesShares * -x.NumShares);
                portfolio.Date = iInvestmentProductsPricesService.GetLastValueDate(investmentProducts);
                portfolio.Prices = iInvestmentProductsPricesService.GetActualPrice(investmentProducts);

                listPortFolio.Add(portfolio);
            }
            return listPortFolio;
        }

        private async Task<decimal?> GetNumShares(InvestmentProducts? investmentProducts)
        {
            var res = await Task.Run(() => iTransactionsArchivedService.GetByInvestmentProduct(investmentProducts));
            res.AddRange(await Task.Run(() => iTransactionsService.GetByInvestmentProduct(investmentProducts)));
            return res?.Sum(x => -x.NumShares);
        }

        private async Task<IOrderedEnumerable<Transactions?>?> GetBuyOperations(InvestmentProducts? investmentProducts)
        {
            var res = await Task.Run(() => iTransactionsArchivedService.GetByInvestmentProduct(investmentProducts)?.Where(x => x.NumShares < 0).ToHashSet());
            res.AddRange(await Task.Run(() => iTransactionsService.GetByInvestmentProduct(investmentProducts)?.Where(x => x.NumShares < 0).ToHashSet()));
            return (IOrderedEnumerable<Transactions?>?)(res?.OrderBy(x => x.Orden));
        }

        private async Task<IOrderedEnumerable<Transactions?>?> GetSellOperations(InvestmentProducts? investmentProducts)
        {
            var res = await Task.Run(() => iTransactionsArchivedService.GetByInvestmentProduct(investmentProducts)?.Where(x => x.NumShares > 0).ToHashSet());
            res.AddRange(await Task.Run(() => iTransactionsService.GetByInvestmentProduct(investmentProducts)?.Where(x => x.NumShares > 0).ToHashSet()));
            return (IOrderedEnumerable<Transactions?>?)(res?.OrderBy(x => x.Orden));
        }
    }
}
