using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Models;
using System.Linq.Expressions;
using Dommel;

namespace GARCA.Data.Managers
{
    public class TransactionsArchivedManager : ManagerBase<TransactionsArchived>
    {
        public async override Task<IEnumerable<TransactionsArchived>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<TransactionsArchived, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, TransactionsArchived>();
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByAccount(int id)
        {
            return await iRycContextService.getConnection().SelectAsync<TransactionsArchived>(x => x.AccountsId == id);
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByPerson(int id)
        {
            return await iRycContextService.getConnection().SelectAsync<TransactionsArchived>(x => x.PersonsId==id);
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByAccountOrderByOrdenDesc(int id)
        {
            return (await iRycContextService.getConnection()
                .SelectAsync<TransactionsArchived>(x => id.Equals(x.AccountsId)))?
                .OrderByDescending(x => x.Orden);
        }        
        
        public async Task<IEnumerable<TransactionsArchived>?> GetByInvestmentProduct(int id)
        {
            return await iRycContextService.getConnection()
                .SelectAsync<TransactionsArchived,InvestmentProducts,TransactionsArchived>(x => x.InvestmentProductsId == id);
        }
    }
}
