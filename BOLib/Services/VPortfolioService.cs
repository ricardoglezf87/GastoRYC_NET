using BOLib.Extensions;
using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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


                listPortFolio.Add(portfolio);                
            }
            return listPortFolio;
        }

        public async Task<decimal?> getNumShares(InvestmentProducts? investmentProducts)
        {
            return await Task.Run(() => TransactionsService.Instance.getByInvestmentProduct(investmentProducts)?.Sum(x => x.numShares));
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
