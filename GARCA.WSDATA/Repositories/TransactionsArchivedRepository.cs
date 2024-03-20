using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class TransactionsArchivedRepository : RepositoryBase<TransactionsArchived>
    {
        public override async Task<IEnumerable<TransactionsArchived>?> GetAll()
        {
            return await dbContext.OpenConnection().GetAllAsync<TransactionsArchived, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, TransactionsArchived>();
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByAccount(int id)
        {
            return await dbContext.OpenConnection().SelectAsync<TransactionsArchived>(x => x.AccountsId == id);
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByPerson(int id)
        {
            return await dbContext.OpenConnection().SelectAsync<TransactionsArchived>(x => x.PersonsId == id);
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByAccountOrderByOrdenDesc(int id)
        {
            return (await dbContext.OpenConnection()
                .SelectAsync<TransactionsArchived>(x => id.Equals(x.AccountsId)))?
                .OrderByDescending(x => x.Orden);
        }

        public async Task<IEnumerable<TransactionsArchived>?> GetByInvestmentProduct(int id)
        {
            return await dbContext.OpenConnection()
                .SelectAsync<TransactionsArchived, InvestmentProducts, TransactionsArchived>(x => x.InvestmentProductsId == id);
        }
    }
}
