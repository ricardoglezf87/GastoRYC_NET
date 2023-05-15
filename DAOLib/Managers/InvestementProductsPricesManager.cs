using DAOLib.Models;
using DAOLib.Services;
using System;
using System.Linq;

namespace DAOLib.Managers
{
    public class InvestmentProductsPricesManager : ManagerBase<InvestmentProductsPricesDAO>
    {
        public bool exists(int? investmentProductId, DateTime? date)
        {
            return RYCContextServiceDAO.getInstance().BBDD.investmentProductsPrices?
                .Any(x => investmentProductId.Equals(x.investmentProductsid) && date.Equals(x.date)) ?? false;
        }

        public Decimal? getActualPrice(InvestmentProductsDAO investmentProducts)
        {
            var query = RYCContextServiceDAO.getInstance()?.BBDD?.investmentProductsPrices?.Where(x => x.investmentProductsid.Equals(investmentProducts.id));
            return query?.Where(x => x.investmentProductsid.Equals(investmentProducts.id)
                && x.date.Equals(query.Max(y => y.date))).Select(z => z.prices).FirstOrDefault();
        }
    }
}
