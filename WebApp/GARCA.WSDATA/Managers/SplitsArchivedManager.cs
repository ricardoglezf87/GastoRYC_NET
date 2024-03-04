
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Managers
{
    public class SplitsArchivedManager : ManagerBase<SplitsArchived>
    {
        public async Task<IEnumerable<SplitsArchived>?> GetbyTransactionid(int transactionid)
        {
            return await dbContext.OpenConnection().
                SelectAsync<SplitsArchived>(x => x.TransactionsId == transactionid);
        }
    }
}
