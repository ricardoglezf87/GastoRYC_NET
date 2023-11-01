using GARCA.DAO.Models;
using GARCA.DAO.Repositories;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class ExpirationsRemindersManager : ManagerBase<ExpirationsRemindersDao>
    {
#pragma warning disable CS8603
        protected override Expression<Func<ExpirationsRemindersDao, object>>[] GetIncludes()
        {
            return new Expression<Func<ExpirationsRemindersDao, object>>[]
            {
                a => a.TransactionsReminders,
                a => a.TransactionsReminders.Person,
                a => a.TransactionsReminders.Account,
                a => a.TransactionsReminders.Category,
                a => a.TransactionsReminders.Category.CategoriesTypes
            };
        }
#pragma warning restore CS8603

        public bool ExistsExpiration(TransactionsRemindersDao? transactionsReminder, DateTime? date)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<ExpirationsRemindersDao>();
                return transactionsReminder != null
                    && (GetEntyWithInclude(repository)?.Any(x => x.TransactionsRemindersid == transactionsReminder.Id && x.Date == date) ?? false);
            }
        }

        public IEnumerable<ExpirationsRemindersDao>? GetByTransactionReminderid(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<ExpirationsRemindersDao>();
                var query = GetEntyWithInclude(repository)?.Where(x => id.Equals(x.TransactionsRemindersid));

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
