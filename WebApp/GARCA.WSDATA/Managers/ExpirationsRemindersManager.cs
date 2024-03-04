
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Managers
{
    public class ExpirationsRemindersManager : ManagerBase<ExpirationsReminders>
    {
        public override async Task<IEnumerable<ExpirationsReminders>?> GetAll()
        {
            return await dbContext.OpenConnection().GetAllAsync<ExpirationsReminders, TransactionsReminders, ExpirationsReminders>(
                (expirationsReminders, transactionsReminders) =>
                {
                    expirationsReminders.TransactionsReminders = transactionsReminders;
                    //expirationsReminders.TransactionsReminders.Categories = iCategoriesService.GetById(transactionsReminders.CategoriesId ?? -99).Result;
                    return expirationsReminders;
                });
        }

        public async Task<bool> ExistsExpiration(TransactionsReminders transactionsReminder, DateTime date)
        {
            return await dbContext.OpenConnection().SelectAsync<ExpirationsReminders>(
                x => x.TransactionsRemindersId == transactionsReminder.Id && x.Date == date) != null;

        }

        public async Task<IEnumerable<ExpirationsReminders>?> GetByTransactionReminderid(int id)
        {
            return await dbContext.OpenConnection().SelectAsync<ExpirationsReminders>(x => id.Equals(x.TransactionsRemindersId));
        }
    }
}
