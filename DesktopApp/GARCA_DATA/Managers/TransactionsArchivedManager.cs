using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Models;
using System.Linq.Expressions;
using Dommel;

namespace GARCA.Data.Managers
{
    public class TransactionsArchivedManager : ManagerBase<TransactionsArchived>
    {

//#pragma warning disable CS8603
//        protected override Expression<Func<TransactionsArchived, object>>[] GetIncludes()
//        {
//            return new Expression<Func<TransactionsArchived, object>>[]
//            {
//                a => a.Account,
//                a => a.Person,
//                a => a.Category,
//                a => a.Tag,
//                a => a.InvestmentProducts,
//                a => a.TransactionStatus
//            };
//        }
//#pragma warning restore CS8603

        public async Task<IEnumerable<TransactionsArchived>?> GetByAccount(int id)
        {
            return await iRycContextService.getConnection().SelectAsync<TransactionsArchived>(x => id.Equals(x.Accountid));
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByPerson(int id)
        {
            return await iRycContextService.getConnection().SelectAsync<TransactionsArchived>(x => id.Equals(x.Personid));
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByAccountOrderByOrdenDesc(int id)
        {
            return (await iRycContextService.getConnection()
                .SelectAsync<TransactionsArchived>(x => id.Equals(x.Accountid)))?
                .OrderByDescending(x => x.Orden);
        }        
        
        public async Task<IEnumerable<TransactionsArchived>?> GetByInvestmentProduct(int id)
        {
            return await iRycContextService.getConnection()
                .SelectAsync<TransactionsArchived>(x => id.Equals(x.InvestmentProductsid));
        }
    }
}
