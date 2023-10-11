using GARCA.DAO.Models;
using System;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class TransactionsRemindersManager : ManagerBase<TransactionsRemindersDao>
    {
#pragma warning disable CS8603
        protected override Expression<Func<TransactionsRemindersDao, object>>[] GetIncludes()
        {
            return new Expression<Func<TransactionsRemindersDao, object>>[]
            {
                a => a.PeriodsReminders,
                a => a.Account,
                a => a.Person,
                a => a.Category,
                a => a.Tag,
                a => a.TransactionStatus
            };
        }
#pragma warning restore CS8603
    }
}
