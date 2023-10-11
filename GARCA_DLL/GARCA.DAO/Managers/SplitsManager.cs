using GARCA.DAO.Models;
using GARCA.DAO.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class SplitsManager : ManagerBase<SplitsDao>
    {
#pragma warning disable CS8603
        protected override Expression<Func<SplitsDao, object>>[] GetIncludes()
        {
            return new Expression<Func<SplitsDao, object>>[]
            {
                a => a.Transaction,
                a => a.Category,
                a => a.Tag
            };
        }
#pragma warning restore CS8603

        public IEnumerable<SplitsDao>? GetbyTransactionidNull()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsDao>();
                var query = GetEntyWithInclude(repository)?.Where(x => x.Transactionid == null);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<SplitsDao>? GetbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsDao>();
                var query = GetEntyWithInclude(repository)?.Where(x => x.Transactionid == transactionid);

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
