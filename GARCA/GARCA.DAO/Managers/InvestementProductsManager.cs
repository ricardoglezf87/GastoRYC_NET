using GARCA.DAO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class InvestmentProductsManager : ManagerBase<InvestmentProductsDAO>
    {
#pragma warning disable CS8603
        protected override Expression<Func<InvestmentProductsDAO, object>>[] GetIncludes()
        {
            return new Expression<Func<InvestmentProductsDAO, object>>[]
            {
                a => a.investmentProductsTypes
            };
        }
#pragma warning restore CS8603

        public IEnumerable<InvestmentProductsDAO>? GetAllOpened()
        {
            return GetAll()?.Where(x => x.active.HasValue && x.active.Value);
        }
    }
}
