using GARCA.DAO.Models;
using GARCA.DAO.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GARCA.DAO.Managers
{
    public class ExpirationsRemindersManager : ManagerBase<ExpirationsRemindersDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<ExpirationsRemindersDAO, object>>[] getIncludes()
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

        public bool existsExpiration(TransactionsRemindersDAO? transactionsReminder, DateTime? date)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<ExpirationsRemindersDAO>();
                return transactionsReminder != null
                    && (getEntyWithInclude(repository)?.Any(x => x.transactionsRemindersid == transactionsReminder.id && x.date == date) ?? false);
            }
        }

        public IEnumerable<ExpirationsRemindersDAO>? getByTransactionReminderid(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<ExpirationsRemindersDAO>();
                var query = getEntyWithInclude(repository)?.Where(x => id.Equals(x.transactionsRemindersid));

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
