using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class SplitsRemindersManager : ManagerBase<SplitsReminders>
    {
        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionidNull()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<SplitsReminders>(x => x.TransactionsId == null);
            }
        }

        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionid(int transactionid)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<SplitsReminders>(x => x.TransactionsId == transactionid);
            }
        }
    }
}
