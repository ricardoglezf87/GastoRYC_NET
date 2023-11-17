
using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Models;
using System.Linq.Expressions;
using Dommel;

namespace GARCA.Data.Managers
{
    public class SplitsArchivedManager : ManagerBase<SplitsArchived>
    {
        public async Task<IEnumerable<SplitsArchived>?> GetbyTransactionid(int transactionid)
        {
            return await iRycContextService.getConnection().
                SelectAsync<SplitsArchived>(x => x.Transactionid == transactionid);
        }
    }
}
