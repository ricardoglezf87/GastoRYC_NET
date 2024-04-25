using Dapper;
using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class TransactionsManager : ManagerBase<Transactions>
    {
        public override async Task<IEnumerable<Transactions>?> GetAll()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.GetAllAsync<Transactions, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, Transactions>();
            }
        }

        public async Task<IEnumerable<Transactions>?> GetByAccount(int accountId)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<Transactions, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, Transactions>
                    (x=>x.AccountsId == accountId);
            }
        }

        public async Task<int> GetNextId()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.ExecuteScalarAsync<int>("SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactions';");
            }
        }

        public async Task UpdateBalance(int id)
        {
            using (var connection = iRycContextService.getConnection())
            {
                await connection.ExecuteAsync(@$"
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
}
