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
    public class SplitsRemindersManager : ManagerBase<SplitsRemindersDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<SplitsRemindersDAO, object>>[] getIncludes()
        {
            return new Expression<Func<SplitsRemindersDAO, object>>[]
            {
                a => a.transaction,
                a => a.category,
                a => a.tag
            };
        }
#pragma warning restore CS8603

        public IEnumerable<SplitsRemindersDAO>? getbyTransactionidNull()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsRemindersDAO>();
                var query = getEntyWithInclude(repository)?.Where(x => x.transactionid == null).ToList();

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<SplitsRemindersDAO>? getbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsRemindersDAO>();
                var query = getEntyWithInclude(repository)?.Where(x => x.transactionid == transactionid).ToList();

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
