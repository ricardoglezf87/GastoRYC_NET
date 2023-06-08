using BOLib.Extensions;
using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace BOLib.Services
{
    public class VPortfolioService
    {
        private static VPortfolioService? _instance;
        private static readonly object _lock = new();

        public static VPortfolioService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new VPortfolioService();
                    }
                }
                return _instance;
            }
        }

        public async Task<List<VPortfolio?>?> getAllAsync()
        {
            List<VPortfolio?>? listPortFolio = new();
            foreach (InvestmentProducts? investmentProducts in 
                await InvestmentProductsService.Instance.getAllOpened()) {
                
                VPortfolio portfolio = new();
                portfolio.id = investmentProducts.id;
                portfolio.description = investmentProducts.description;
                portfolio.symbol = investmentProducts.symbol;
                portfolio.numShares = await getNumShares(investmentProducts);

                List<Transactions?>? lBuy = await getBuyOperations(investmentProducts);
                List<Transactions?>? lSell = await getSellOperations(investmentProducts);

                foreach(Transactions? sell in lSell) 
                {
                    decimal? shares = sell.numShares;
                    if (shares != null && shares > 0)
                    {
                        foreach (Transactions? buy in lBuy)
                        {
                            if(buy.numShares != null && buy.numShares != 0 )
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
                portfolio.date = InvestmentProductsPricesService.Instance.getLastValueDate(investmentProducts);
                portfolio.prices = InvestmentProductsPricesService.Instance.getActualPrice(investmentProducts);                

                listPortFolio.Add(portfolio);                
            }
            return listPortFolio;
        }

        public async Task<decimal?> getNumShares(InvestmentProducts? investmentProducts)
        {
            return await Task.Run(() => TransactionsService.Instance.getByInvestmentProduct(investmentProducts)?.Sum(x => -x.numShares));
        }

        public async Task<List<Transactions?>?> getBuyOperations(InvestmentProducts? investmentProducts)
        {
            return await Task.Run(() => TransactionsService.Instance.getByInvestmentProduct(investmentProducts)?.Where(x => x.numShares < 0).OrderBy(x => x.date).ToList());
        }

        public async Task<List<Transactions?>?> getSellOperations(InvestmentProducts? investmentProducts)
        {
            return await Task.Run(() => TransactionsService.Instance.getByInvestmentProduct(investmentProducts)?.Where(x => x.numShares > 0).OrderBy(x => x.date).ToList());
        }
    }
}
