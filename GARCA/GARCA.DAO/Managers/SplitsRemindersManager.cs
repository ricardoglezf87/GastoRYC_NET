using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public List<SplitsRemindersDAO>? getbyTransactionidNull()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsRemindersDAO>();
                return getEntyWithInclude(repository)?.Where(x => x.transactionid == null).ToList();
            }
        }

        public List<SplitsRemindersDAO>? getbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsRemindersDAO>();
                return getEntyWithInclude(repository)?.Where(x => x.transactionid == transactionid).ToList();
            }
        }
    }
}
