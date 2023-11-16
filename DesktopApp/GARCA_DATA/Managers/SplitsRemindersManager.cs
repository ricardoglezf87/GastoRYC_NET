using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Models;
using System.Linq.Expressions;
using Dommel;

namespace GARCA.Data.Managers
{
    public class SplitsRemindersManager : ManagerBase<SplitsReminders, Int32>
    {
//#pragma warning disable CS8603
//        protected override Expression<Func<SplitsReminders, object>>[] GetIncludes()
//        {
//            return new Expression<Func<SplitsReminders, object>>[]
//            {
//                a => a.Transaction,
//                a => a.Category,
//                a => a.Tag
//            };
//        }
//#pragma warning restore CS8603

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
