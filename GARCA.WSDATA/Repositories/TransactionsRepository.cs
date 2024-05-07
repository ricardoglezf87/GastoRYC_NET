using Dapper;
using Dommel;
using GARCA.Models;
using GARCA.Utils.Extensions;
using System.Data;


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

        public override async Task<Transactions> Save(Transactions obj)
        {
            obj.Date.RemoveTime();
            Transactions transaction = await base.Save(obj);
            await postChange(obj);
            return transaction;
        }

        public override async Task<bool> Delete(Transactions obj)
        {
            bool result = await Delete(obj.Id);
            await postChange(obj);
            return result;
        }


        private async Task postChange(Transactions obj)
        {
            await UpdateTranfer(obj.Id);
            await UpdateBalance(obj.Date ?? DateTime.Now);
            _ = UpdateDefaultPerson(obj.PersonsId ?? -99);
        }

        public async Task<IEnumerable<Transactions>?> GetByAccount(int accountId)
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.SelectAsync<Transactions, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, Transactions>
                    (x => x.AccountsId == accountId);
            }
        }

        public async Task UpdateTranfer(int id)
        {
            using (var connection = dbContext.OpenConnection())
            {
                await connection.ExecuteAsync("UpdateTranfer", new { Tid = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateBalance(DateTime transactionDate)
        {
            using (var connection = dbContext.OpenConnection())
            {
                await connection.ExecuteAsync("UpdateBalancebyDate", new { p_transaction_date = transactionDate }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateDefaultPerson(int personId)
        {
            using (var connection = dbContext.OpenConnection())
            {
                await connection.ExecuteAsync("UpdatePersonsCategoriesId", new { person_id = personId }, commandType: CommandType.StoredProcedure);
            }
        }        
    }
}
