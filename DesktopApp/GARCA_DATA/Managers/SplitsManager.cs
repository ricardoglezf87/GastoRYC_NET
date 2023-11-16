
using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Models;
using System.Linq.Expressions;
using Dommel;

namespace GARCA.Data.Managers
{
    public class SplitsManager : ManagerBase<Splits>
    {
//#pragma warning disable CS8603
//        protected override Expression<Func<Splits, object>>[] GetIncludes()
//        {
//            return new Expression<Func<Splits, object>>[]
//            {
//                a => a.Transaction,
//                a => a.Category,
//                a => a.Tag
//            };
//        }
//#pragma warning restore CS8603

        public async Task<IEnumerable<Splits>?> GetbyTransactionidNull()
        {
            return await iRycContextService.getConnection().SelectAsync<Splits>(x => x.Transactionid == null);
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionid(int transactionid)
        {
            return await iRycContextService.getConnection().SelectAsync<Splits>(x => x.Transactionid == transactionid);
        }
    }
}
