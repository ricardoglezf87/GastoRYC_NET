
using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class SplitsArchivedManager : ManagerBase<SplitsArchived>
    {
        public async Task<IEnumerable<SplitsArchived>?> GetbyTransactionid(int transactionid)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<SplitsArchived>(x => x.TransactionsId == transactionid);
            }
        }
    }
}
