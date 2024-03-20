
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class ExpirationsRemindersRepository : RepositoryBase<ExpirationsReminders>
    {
        public override async Task<IEnumerable<ExpirationsReminders>?> GetAll()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.GetAllAsync<ExpirationsReminders, TransactionsReminders, ExpirationsReminders>(
                (expirationsReminders, transactionsReminders) =>
                {
                    expirationsReminders.TransactionsReminders = transactionsReminders;
                    //expirationsReminders.TransactionsReminders.Categories = iCategoriesService.GetById(transactionsReminders.CategoriesId ?? -99).Result;
                    return expirationsReminders;
                });
            }
        }

        public async Task<bool> ExistsExpiration(TransactionsReminders transactionsReminder, DateTime date)
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.SelectAsync<ExpirationsReminders>(
                x => x.TransactionsRemindersId == transactionsReminder.Id && x.Date == date) != null;
            }
        }

        public async Task<IEnumerable<ExpirationsReminders>?> GetByTransactionReminderid(int id)
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.SelectAsync<ExpirationsReminders>(x => id.Equals(x.TransactionsRemindersId));
            }
        }
    }
}
