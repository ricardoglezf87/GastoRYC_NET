using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class InvestmentProductsPricesManager : ManagerBase<InvestmentProductsPricesDAO>
    {
#pragma warning disable CS8603
        protected override Expression<Func<InvestmentProductsPricesDAO, object>>[] GetIncludes()
        {
            return new Expression<Func<InvestmentProductsPricesDAO, object>>[]
            {
                a => a.investmentProducts
            };
        }
#pragma warning restore CS8603

        public bool Exists(int? investmentProductId, DateTime? date)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPricesDAO>();
                return GetEntyWithInclude(repository)?
                    .Any(x => investmentProductId.Equals(x.investmentProductsid) && date.Equals(x.date)) ?? false;
            }
        }

        public Decimal? GetActualPrice(InvestmentProductsDAO investmentProducts)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPricesDAO>();
                var query = GetEntyWithInclude(repository)?.Where(x => x.investmentProductsid.Equals(investmentProducts.id));
                return query?.Where(x => x.investmentProductsid.Equals(investmentProducts.id)
                    && x.date.Equals(query.Max(y => y.date))).Select(z => z.prices).FirstOrDefault();
            }
        }

        public DateTime? GetLastValueDate(InvestmentProductsDAO investmentProducts)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPricesDAO>();
                return GetEntyWithInclude(repository)?.Where(x => x.investmentProductsid.Equals(investmentProducts.id)).Max(x => x.date);
            }
        }
    }
}
