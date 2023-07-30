using GARCA.DAO.Models;
using GARCA.DAO.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class SplitsRemindersManager : ManagerBase<SplitsRemindersDAO>
    {
#pragma warning disable CS8603
        protected override Expression<Func<SplitsRemindersDAO, object>>[] GetIncludes()
        {
            return new Expression<Func<SplitsRemindersDAO, object>>[]
            {
                a => a.transaction,
                a => a.category,
                a => a.tag
            };
        }
#pragma warning restore CS8603

        public IEnumerable<SplitsRemindersDAO>? GetbyTransactionidNull()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsRemindersDAO>();
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

        public IEnumerable<SplitsRemindersDAO>? GetbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsRemindersDAO>();
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
