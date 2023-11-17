using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Models;
using System.Linq.Expressions;
using Dommel;

namespace GARCA.Data.Managers
{
    public class TransactionsArchivedManager : ManagerBase<TransactionsArchived>
    {
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
