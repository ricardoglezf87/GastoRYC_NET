using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GARCA.DAO.Managers
{
    public class SplitsManager : ManagerBase<SplitsDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<SplitsDAO, object>>[] getIncludes()
        {
            return new Expression<Func<SplitsDAO, object>>[]
            {
                a => a.transaction,
                a => a.category,
                a => a.tag
            };
        }
#pragma warning restore CS8603

        public IEnumerable<SplitsDAO>? getbyTransactionidNull()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsDAO>();
                var query = getEntyWithInclude(repository)?.Where(x => x.transactionid == null);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<SplitsDAO>? getbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsDAO>();
                var query = getEntyWithInclude(repository)?.Where(x => x.transactionid == transactionid);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
