using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Managers
{
    public class SplitsRemindersManager : ManagerBase<SplitsReminders>
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
