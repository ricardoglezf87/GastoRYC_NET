
using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Models;
using System.Linq.Expressions;
using Dommel;

namespace GARCA.Data.Managers
{
    public class SplitsArchivedManager : ManagerBase<SplitsArchived, Int32>
    {
//#pragma warning disable CS8603
//        protected override Expression<Func<SplitsArchived, object>>[] GetIncludes()
//        {
//            return new Expression<Func<SplitsArchived, object>>[]
//            {
//                a => a.Transaction,
//                a => a.Category,
//                a => a.Tag
//            };
//        }
//#pragma warning restore CS8603

        public async Task<IEnumerable<SplitsArchived>?> GetbyTransactionid(int transactionid)
        {
            return await iRycContextService.getConnection().
                SelectAsync<SplitsArchived>(x => x.Transactionid == transactionid);
        }
    }
}
