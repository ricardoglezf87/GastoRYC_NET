
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class SplitsRepository : RepositoryBase<Splits>
    {
        public async Task<IEnumerable<Splits>?> GetbyTransactionidNull()
        {
            return await dbContext.OpenConnection().SelectAsync<Splits>(x => x.TransactionsId == null);
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionid(int transactionid)
        {
            return await dbContext.OpenConnection().SelectAsync<Splits>(x => x.TransactionsId == transactionid);
        }
    }
}
