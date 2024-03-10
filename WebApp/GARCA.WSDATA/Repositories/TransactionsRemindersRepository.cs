using Dommel;
using GARCA.Models;



namespace GARCA.wsData.Repositories
{
    public class TransactionsRemindersRepository : RepositoryBase<TransactionsReminders>
    {
        public override async Task<IEnumerable<TransactionsReminders>?> GetAll()
        {
            return await dbContext.OpenConnection().GetAllAsync<TransactionsReminders,
                PeriodsReminders, Accounts, Categories, TransactionsStatus, Persons, Tags, TransactionsReminders>();
        }
    }
}
