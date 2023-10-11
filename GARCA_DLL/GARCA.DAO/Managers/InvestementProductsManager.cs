using GARCA.DAO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class InvestmentProductsManager : ManagerBase<InvestmentProductsDao>
    {
#pragma warning disable CS8603
        protected override Expression<Func<InvestmentProductsDao, object>>[] GetIncludes()
        {
            return new Expression<Func<InvestmentProductsDao, object>>[]
            {
                a => a.InvestmentProductsTypes
            };
        }
#pragma warning restore CS8603

        public IEnumerable<InvestmentProductsDao>? GetAllOpened()
        {
            return GetAll()?.Where(x => x.Active.HasValue && x.Active.Value);
        }
    }
}
