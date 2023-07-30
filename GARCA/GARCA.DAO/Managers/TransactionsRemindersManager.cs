using GARCA.DAO.Models;
using System;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class TransactionsRemindersManager : ManagerBase<TransactionsRemindersDAO>
    {
#pragma warning disable CS8603
        protected override Expression<Func<TransactionsRemindersDAO, object>>[] GetIncludes()
        {
            return new Expression<Func<TransactionsRemindersDAO, object>>[]
            {
                a => a.periodsReminders,
                a => a.account,
                a => a.person,
                a => a.category,
                a => a.tag,
                a => a.transactionStatus
            };
        }
#pragma warning restore CS8603
    }
}
