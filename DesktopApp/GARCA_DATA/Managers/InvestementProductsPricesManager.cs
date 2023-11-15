
using GARCA.Models;
using System.Linq.Expressions;

namespace GARCA.Data.Managers
{
    public class InvestmentProductsPricesManager : ManagerBase<InvestmentProductsPrices, Int32>
    {
#pragma warning disable CS8603
        protected override Expression<Func<InvestmentProductsPrices, object>>[] GetIncludes()
        {
            return new Expression<Func<InvestmentProductsPrices, object>>[]
            {
                a => a.InvestmentProducts
            };
        }
#pragma warning restore CS8603

        public bool Exists(int? investmentProductId, DateTime? date)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPrices>();
                return GetEntyWithInclude(repository)?
                    .Any(x => investmentProductId.Equals(x.InvestmentProductsid) && date.Equals(x.Date)) ?? false;
            }
        }

        public Decimal? GetActualPrice(InvestmentProducts investmentProducts)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPrices>();
                var query = GetEntyWithInclude(repository)?.Where(x => x.InvestmentProductsid.Equals(investmentProducts.Id));
                return query?.Where(x => x.InvestmentProductsid.Equals(investmentProducts.Id)
                    && x.Date.Equals(query.Max(y => y.Date))).Select(z => z.Prices).FirstOrDefault();
            }
        }

        public DateTime? GetLastValueDate(InvestmentProducts investmentProducts)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPrices>();
                return GetEntyWithInclude(repository)?.Where(x => x.InvestmentProductsid.Equals(investmentProducts.Id)).Max(x => x.Date);
            }
        }
    }
}
