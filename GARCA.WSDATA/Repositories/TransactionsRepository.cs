using Dapper;
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class TransactionsRepository : RepositoryBase<Transactions>
    {
        public override async Task<IEnumerable<Transactions>?> GetAll()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.GetAllAsync<Transactions, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, Transactions>();
            }
        }

        public async Task<int> GetNextId()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.ExecuteScalarAsync<int>("SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactions';");
            }
        }

        public async Task UpdateBalance(int id)
        {
            using (var connection = dbContext.OpenConnection())
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
