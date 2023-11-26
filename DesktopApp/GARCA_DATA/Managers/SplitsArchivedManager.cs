
using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class SplitsArchivedManager : ManagerBase<SplitsArchived>
    {
        public async Task<IEnumerable<SplitsArchived>?> GetbyTransactionid(int transactionid)
        {
            return await iRycContextService.getConnection().
                SelectAsync<SplitsArchived>(x => x.TransactionsId == transactionid);
        }
    }
}
