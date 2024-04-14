
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class SplitsRepository : RepositoryBase<Splits>
    {
        public async Task<IEnumerable<Splits>?> GetbyTransactionidNull()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.SelectAsync<Splits>(x => x.TransactionsId == null);
            }
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionid(int transactionid)
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.SelectAsync<Splits>(x => x.TransactionsId == transactionid);
            }
        }
    }
}
