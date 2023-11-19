using Dapper;
using Dommel;
using GARCA.Models;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class TransactionsManager : ManagerBase<Transactions>
    {
        public async override Task<IEnumerable<Transactions>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<Transactions, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, Transactions>();
        }

        public async Task<int> GetNextId()
        {
            return await iRycContextService.getConnection().ExecuteScalarAsync<int>("SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactions';");
        }
    }
}
