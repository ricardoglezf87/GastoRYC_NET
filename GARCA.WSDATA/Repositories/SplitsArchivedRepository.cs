
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class SplitsArchivedRepository : RepositoryBase<SplitsArchived>
    {
        public async Task<IEnumerable<SplitsArchived>?> GetbyTransactionid(int transactionid)
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.SelectAsync<SplitsArchived>(x => x.TransactionsId == transactionid);
            }
        }
    }
}
