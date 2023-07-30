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
        public override Expression<Func<SplitsDAO, object>>[] GetIncludes()
        {
            return new Expression<Func<SplitsDAO, object>>[]
            {
                a => a.transaction,
                a => a.category,
                a => a.tag
            };
        }
#pragma warning restore CS8603

        public IEnumerable<SplitsDAO>? GetbyTransactionidNull()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsDAO>();
                var query = GetEntyWithInclude(repository)?.Where(x => x.transactionid == null);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<SplitsDAO>? GetbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsDAO>();
                var query = GetEntyWithInclude(repository)?.Where(x => x.transactionid == transactionid);

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
