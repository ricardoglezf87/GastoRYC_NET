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

        public async Task UpdateBalance(int id)
        {
            await iRycContextService.getConnection().ExecuteAsync(@$"
                update transactions
                set
	                balance =(select round(sum(t2.amountIn-t2.amountOut),2)
			                from transactions t2
			                where t2.accountid = transactions.accountid
				                and t2.orden<=transactions.orden) 
                where accountid = {id}
            ");
        }
    }
}
