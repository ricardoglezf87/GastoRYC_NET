using Dommel;
using GARCA.Models;



namespace GARCA.wsData.Repositories
{
    public class TransactionsRemindersRepository : RepositoryBase<TransactionsReminders>
    {
        public override async Task<IEnumerable<TransactionsReminders>?> GetAll()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.GetAllAsync<TransactionsReminders,
                PeriodsReminders, Accounts, Categories, TransactionsStatus, Persons, Tags, TransactionsReminders>();
            }
        }
    }
}
