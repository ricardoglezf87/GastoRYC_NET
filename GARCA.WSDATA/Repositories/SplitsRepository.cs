using Dapper;
using Dommel;
using GARCA.Models;
using System.Data;

namespace GARCA.wsData.Repositories
{
    public class SplitsRepository : RepositoryBase<Splits>
    {
        public override async Task<Splits> Save(Splits obj)
        {
            obj = await base.Save(obj);
            await postChange(obj);
            return await GetById(obj.Id) ?? obj;
        }

        public override async Task<bool> Delete(Splits obj)
        {
            bool result = await base.Delete(obj);
            await postChange(obj);
            return result;
        }

        private async Task postChange(Splits obj)
        {
            await UpdateTransactionWithSplits(obj.TransactionsId ?? -99);
            await UpdateTranferSplit(obj.Id);            
            await UpdateTransactionBalance(obj.TransactionsId ?? -99);
        }

        public async Task UpdateTranferSplit(int id)
        {
            using (var connection = dbContext.OpenConnection())
            {
                await connection.ExecuteAsync("UpdateTranferSplit", new { Sid = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateTransactionWithSplits(int id)
        {
            using (var connection = dbContext.OpenConnection())
            {
                await connection.ExecuteAsync("UpdateTransactionWithSplit", new { Tid = id }, commandType: CommandType.StoredProcedure);
            }
        }

        private async Task UpdateTransactionBalance(int id)
        {
            using (var connection = dbContext.OpenConnection())
            {
                await connection.ExecuteAsync("UpdateBalancebyId", new { Tid = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionidNull()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.SelectAsync<Splits>(x => x.TransactionsId == null);
            }
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionid(int transactionid)
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.SelectAsync<Splits>(x => x.TransactionsId == transactionid);
            }
        }              
    }
}
