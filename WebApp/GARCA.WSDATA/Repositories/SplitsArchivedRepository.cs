
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class SplitsArchivedRepository : RepositoryBase<SplitsArchived>
    {
        public async Task<IEnumerable<SplitsArchived>?> GetbyTransactionid(int transactionid)
        {
            return await dbContext.OpenConnection().
                SelectAsync<SplitsArchived>(x => x.TransactionsId == transactionid);
        }
    }
}
