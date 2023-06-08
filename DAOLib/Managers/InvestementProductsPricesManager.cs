using DAOLib.Models;
using DAOLib.Repositories;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace DAOLib.Managers
{
    public class InvestmentProductsPricesManager : ManagerBase<InvestmentProductsPricesDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<InvestmentProductsPricesDAO, object>>[] getIncludes()
        {
            return new Expression<Func<InvestmentProductsPricesDAO, object>>[]
            {
                a => a.investmentProducts
            };
        }
#pragma warning restore CS8603

        public bool exists(int? investmentProductId, DateTime? date)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPricesDAO>();
                return getEntyWithInclude(repository)?
                    .Any(x => investmentProductId.Equals(x.investmentProductsid) && date.Equals(x.date)) ?? false;
            }
        }

        public Decimal? getActualPrice(InvestmentProductsDAO investmentProducts)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPricesDAO>();
                var query = getEntyWithInclude(repository)?.Where(x => x.investmentProductsid.Equals(investmentProducts.id));
                return query?.Where(x => x.investmentProductsid.Equals(investmentProducts.id)
                    && x.date.Equals(query.Max(y => y.date))).Select(z => z.prices).FirstOrDefault();
            }
        }

        public DateTime? getLastValueDate(InvestmentProductsDAO investmentProducts)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsPricesDAO>();
                return getEntyWithInclude(repository)?.Where(x => x.investmentProductsid.Equals(investmentProducts.id))?.Max(x => x.date);
            }
        }
    }
}
