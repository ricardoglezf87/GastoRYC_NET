using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class SplitsRemindersRepository : RepositoryBase<SplitsReminders>
    {
        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionidNull()
        {
            return await dbContext.OpenConnection().SelectAsync<SplitsReminders>(x => x.TransactionsId == null);
        }

        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionid(int transactionid)
        {
            return await dbContext.OpenConnection().SelectAsync<SplitsReminders>(x => x.TransactionsId == transactionid);
        }
    }
}
