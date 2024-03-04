
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Managers
{
    public class SplitsManager : ManagerBase<Splits>
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
