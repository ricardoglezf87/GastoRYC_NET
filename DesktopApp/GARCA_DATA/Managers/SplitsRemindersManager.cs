using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Models;
using System.Linq.Expressions;
using Dommel;

namespace GARCA.Data.Managers
{
    public class SplitsRemindersManager : ManagerBase<SplitsReminders>
    {
       public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionidNull()
        {
            return await iRycContextService.getConnection().SelectAsync<SplitsReminders>(x => x.Transactionid == null);
        }

        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionid(int transactionid)
        {
                return await iRycContextService.getConnection().SelectAsync<SplitsReminders>(x => x.Transactionid == transactionid);          
        }
    }
}
