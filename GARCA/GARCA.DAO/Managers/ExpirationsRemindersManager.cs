using GARCA.DAO.Models;
using GARCA.DAO.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class ExpirationsRemindersManager : ManagerBase<ExpirationsRemindersDAO>
    {
#pragma warning disable CS8603
        protected override Expression<Func<ExpirationsRemindersDAO, object>>[] GetIncludes()
        {
            return new Expression<Func<ExpirationsRemindersDAO, object>>[]
            {
                a => a.transactionsReminders,
                a => a.transactionsReminders.person,
                a => a.transactionsReminders.account,
                a => a.transactionsReminders.category,
                a => a.transactionsReminders.category.categoriesTypes
            };
        }
#pragma warning restore CS8603

        public bool ExistsExpiration(TransactionsRemindersDAO? transactionsReminder, DateTime? date)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<ExpirationsRemindersDAO>();
                return transactionsReminder != null
                    && (GetEntyWithInclude(repository)?.Any(x => x.transactionsRemindersid == transactionsReminder.id && x.date == date) ?? false);
            }
        }

        public IEnumerable<ExpirationsRemindersDAO>? GetByTransactionReminderid(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<ExpirationsRemindersDAO>();
                var query = GetEntyWithInclude(repository)?.Where(x => id.Equals(x.transactionsRemindersid));

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
