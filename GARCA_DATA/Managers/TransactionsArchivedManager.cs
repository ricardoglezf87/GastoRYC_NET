using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class TransactionsArchivedManager : ManagerBase<TransactionsArchived>
    {
        public override async Task<IEnumerable<TransactionsArchived>?> GetAll()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.GetAllAsync<TransactionsArchived, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, TransactionsArchived>();
            }
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByAccount(int id)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<TransactionsArchived>(x => x.AccountsId == id);
            }
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByPerson(int id)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<TransactionsArchived>(x => x.PersonsId == id);
            }
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByAccountOrderByOrdenDesc(int id)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return (await connection
                    .SelectAsync<TransactionsArchived>(x => id.Equals(x.AccountsId)))?
                    .OrderByDescending(x => x.Orden);
            }
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByInvestmentProduct(int id)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<TransactionsArchived, InvestmentProducts, TransactionsArchived>(x => x.InvestmentProductsId == id);
            }
        }
    }
}
