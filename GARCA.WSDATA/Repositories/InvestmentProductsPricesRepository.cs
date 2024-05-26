
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class InvestmentProductsPricesRepository : RepositoryBase<InvestmentProductsPrices>
    {
        public async Task<bool> Exists(int investmentProductId, DateTime date)
        {
            using (var connection = DBContext.OpenConnection())
            {
                return await connection.SelectAsync<InvestmentProductsPrices>(
                x => x.InvestmentProductsid == investmentProductId && x.Date == date) != null;
            }
        }

        public async Task<Decimal?> GetActualPrice(InvestmentProducts investmentProducts)
        {
            using (var connection = DBContext.OpenConnection())
            {
                var query = await connection.SelectAsync<InvestmentProductsPrices>(x => x.InvestmentProductsid == investmentProducts.Id && x.Prices != 0);
                return query?.Where(x => investmentProducts.Id == x.InvestmentProductsid
                        && x.Date.Equals(query.Max(y => y.Date))).Select(z => z.Prices).FirstOrDefault();
            }
        }

        public async Task<DateTime?> GetLastValueDate(InvestmentProducts investmentProducts)
        {
            using (var connection = DBContext.OpenConnection())
            {
                var query = await connection.SelectAsync<InvestmentProductsPrices>(x => x.InvestmentProductsid == investmentProducts.Id && x.Prices != 0);
                return query.Max(x => x.Date);
            }
        }
    }
}
