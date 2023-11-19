
using Dommel;
using GARCA.Models;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class ExpirationsRemindersManager : ManagerBase<ExpirationsReminders>
    {  
        public override async Task<IEnumerable<ExpirationsReminders>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<ExpirationsReminders,TransactionsReminders, ExpirationsReminders>(
                (expirationsReminders, transactionsReminders) => 
                {
                    expirationsReminders.TransactionsReminders = transactionsReminders;
                    expirationsReminders.TransactionsReminders.Categories = iCategoriesService.GetById(transactionsReminders.CategoriesId ?? -99).Result;
                    return expirationsReminders;
                });
        }

        public async Task<bool> ExistsExpiration(TransactionsReminders transactionsReminder, DateTime date)
        {
            return await iRycContextService.getConnection().SelectAsync<ExpirationsReminders>(
                x => x.TransactionsRemindersId == transactionsReminder.Id && x.Date == date) != null;
            
        }

        public async Task<IEnumerable<ExpirationsReminders>?> GetByTransactionReminderid(int id)
        {
            return await iRycContextService.getConnection().SelectAsync<ExpirationsReminders>(x => id.Equals(x.TransactionsRemindersId));           
        }
    }
}
