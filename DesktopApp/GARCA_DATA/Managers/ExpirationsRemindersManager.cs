using GARCA.DAO.Repositories;
using GARCA.Models;
using System.Linq.Expressions;

namespace GARCA.Data.Managers
{
    public class ExpirationsRemindersManager : ManagerBase<ExpirationsReminders>
    {
#pragma warning disable CS8603
        protected override Expression<Func<ExpirationsReminders, object>>[] GetIncludes()
        {
            return new Expression<Func<ExpirationsReminders, object>>[]
            {
                a => a.TransactionsReminders,
                a => a.TransactionsReminders.Person,
                a => a.TransactionsReminders.Account,
                a => a.TransactionsReminders.Category,
                a => a.TransactionsReminders.Category.CategoriesTypes
            };
        }
#pragma warning restore CS8603

        public bool ExistsExpiration(TransactionsReminders? transactionsReminder, DateTime? date)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<ExpirationsReminders>();
                return transactionsReminder != null
                    && (GetEntyWithInclude(repository)?.Any(x => x.TransactionsRemindersid == transactionsReminder.Id && x.Date == date) ?? false);
            }
        }

        public IEnumerable<ExpirationsReminders>? GetByTransactionReminderid(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<ExpirationsReminders>();
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
