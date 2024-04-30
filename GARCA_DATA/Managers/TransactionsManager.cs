using Dapper;
using Dommel;
using GARCA.Models;
using System.Data;
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

        public override async Task<Transactions> Save(Transactions obj)
        {
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
            await UpdateBalance(obj.Date ?? DateTime.Now);
            _ = UpdateDefaultPerson(obj.PersonsId ?? -99);
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
                return await connection.ExecuteScalarAsync<int>(
                    @$"SELECT AUTO_INCREMENT 
                        FROM information_schema.TABLES 
                        WHERE TABLE_SCHEMA = '{DATABASE_NAME}' 
                        AND TABLE_NAME = 'Transactions';");
            }
        }

        public async Task UpdateBalance(DateTime transactionDate)
        {
            using (var connection = iRycContextService.getConnection())
            {
                await connection.ExecuteAsync("UpdateBalancebyDate", new { p_transaction_date = transactionDate }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateDefaultPerson(int personId)
        {
            using (var connection = iRycContextService.getConnection())
            {
                await connection.ExecuteAsync("UpdatePersonsCategoriesId", new { person_id = personId }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
