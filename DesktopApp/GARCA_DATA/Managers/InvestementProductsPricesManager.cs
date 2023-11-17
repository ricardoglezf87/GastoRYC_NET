
using Dommel;
using GARCA.Models;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class InvestmentProductsPricesManager : ManagerBase<InvestmentProductsPrices>
    {
        public async Task<bool> Exists(int investmentProductId, DateTime date)
        {
            return await iRycContextService.getConnection().SelectAsync <InvestmentProductsPrices>(
                x => investmentProductId.Equals(x.InvestmentProductsid) && date.Equals(x.Date)) != null;           
        }

        public async Task<Decimal?> GetActualPrice(InvestmentProducts investmentProducts)
        {
            var query = await iRycContextService.getConnection().SelectAsync<InvestmentProductsPrices>(x => x.InvestmentProductsid.Equals(investmentProducts.Id));
            return query?.Where(x => x.InvestmentProductsid.Equals(investmentProducts.Id)
                    && x.Date.Equals(query.Max(y => y.Date))).Select(z => z.Prices).FirstOrDefault();
        }

        public async Task<DateTime?> GetLastValueDate(InvestmentProducts investmentProducts)
        {
            var query = await iRycContextService.getConnection().SelectAsync<InvestmentProductsPrices>(x => x.InvestmentProductsid.Equals(investmentProducts.Id));
            return query.Max(x => x.Date);
        }
    }
}
