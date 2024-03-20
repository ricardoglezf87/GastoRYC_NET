
using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class SplitsManager : ManagerBase<Splits>
    {
        public async Task<IEnumerable<Splits>?> GetbyTransactionidNull()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<Splits>(x => x.TransactionsId == null);
            }
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionid(int transactionid)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<Splits>(x => x.TransactionsId == transactionid);
            }
        }
    }
}
