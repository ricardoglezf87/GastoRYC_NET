using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class SplitsRemindersRepository : RepositoryBase<SplitsReminders>
    {
        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionidNull()
        {
            using (var connection = DBContext.OpenConnection())
            {
                return await connection.SelectAsync<SplitsReminders>(x => x.TransactionsId == null);
            }
        }

        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionid(int transactionid)
        {
            using (var connection = DBContext.OpenConnection())
            {
                return await connection.SelectAsync<SplitsReminders>(x => x.TransactionsId == transactionid);
            }
        }
    }
}
