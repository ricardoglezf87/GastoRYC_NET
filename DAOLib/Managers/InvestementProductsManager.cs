using DAOLib.Models;
using DAOLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DAOLib.Managers
{
    public class InvestmentProductsManager : ManagerBase<InvestmentProductsDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<InvestmentProductsDAO, object>>[] getIncludes()
        {
            return new Expression<Func<InvestmentProductsDAO, object>>[]
            {
                a => a.investmentProductsTypes
            };
        }
#pragma warning restore CS8603

        public List<InvestmentProductsDAO>? getAllOpened()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsDAO>();
                return getAll()?.Where(x => x.active.HasValue && x.active.Value).ToList();
            }
        }
    }
}
