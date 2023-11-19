
using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Models;
using System.Linq.Expressions;
using Dommel;

namespace GARCA.Data.Managers
{
    public class SplitsManager : ManagerBase<Splits>
    {
        public async Task<IEnumerable<Splits>?> GetbyTransactionidNull()
        {
            return await iRycContextService.getConnection().SelectAsync<Splits>(x => x.TransactionsId == null);
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionid(int transactionid)
        {
            return await iRycContextService.getConnection().SelectAsync<Splits>(x => x.TransactionsId == transactionid);
        }
    }
}
