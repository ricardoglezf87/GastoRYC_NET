using GARCA.DAO.Models;
using GARCA.DAO.Repositories;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class InvestmentProductsPricesManager : ManagerBase<InvestmentProductsPricesDao>
    {
#pragma warning disable CS8603
        protected override Expression<Func<InvestmentProductsPricesDao, object>>[] GetIncludes()
        {
            return new Expression<Func<InvestmentProductsPricesDao, object>>[]
            {
                a => a.InvestmentProducts
            };
        }
#pragma warning restore CS8603

        public bool Exists(int? investmentProductId, DateTime? date)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPricesDao>();
                return GetEntyWithInclude(repository)?
                    .Any(x => investmentProductId.Equals(x.InvestmentProductsid) && date.Equals(x.Date)) ?? false;
            }
        }

        public Decimal? GetActualPrice(InvestmentProductsDao investmentProducts)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPricesDao>();
                var query = GetEntyWithInclude(repository)?.Where(x => x.InvestmentProductsid.Equals(investmentProducts.Id));
                return query?.Where(x => x.InvestmentProductsid.Equals(investmentProducts.Id)
                    && x.Date.Equals(query.Max(y => y.Date))).Select(z => z.Prices).FirstOrDefault();
            }
        }

        public DateTime? GetLastValueDate(InvestmentProductsDao investmentProducts)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPricesDao>();
                return GetEntyWithInclude(repository)?.Where(x => x.InvestmentProductsid.Equals(investmentProducts.Id)).Max(x => x.Date);
            }
        }
    }
}
