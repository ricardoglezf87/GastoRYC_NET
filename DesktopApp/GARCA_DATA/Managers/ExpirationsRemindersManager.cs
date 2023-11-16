
using Dommel;
using GARCA.Models;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class ExpirationsRemindersManager : ManagerBase<ExpirationsReminders, Int32>
    {
//#pragma warning disable CS8603
//        protected override Expression<Func<ExpirationsReminders, object>>[] GetIncludes()
//        {
//            return new Expression<Func<ExpirationsReminders, object>>[]
//            {
//                a => a.TransactionsReminders,
//                a => a.TransactionsReminders.Person,
//                a => a.TransactionsReminders.Account,
//                a => a.TransactionsReminders.Category,
//                a => a.TransactionsReminders.Category.CategoriesTypes
//            };
//        }
//#pragma warning restore CS8603

        public async Task<bool> ExistsExpiration(TransactionsReminders transactionsReminder, DateTime date)
        {
            return await iRycContextService.getConnection().SelectAsync<ExpirationsReminders>(
                x => x.TransactionsRemindersid == transactionsReminder.Id && x.Date == date) != null;
            
        }

        public async Task<IEnumerable<ExpirationsReminders>?> GetByTransactionReminderid(int id)
        {
            return await iRycContextService.getConnection().SelectAsync<ExpirationsReminders>(x => id.Equals(x.TransactionsRemindersid));           
        }
    }
}
