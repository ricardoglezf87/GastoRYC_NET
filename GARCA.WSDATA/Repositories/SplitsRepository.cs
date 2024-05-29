using Dapper;
using Dommel;
using GARCA.Models;
using Newtonsoft.Json;
using System.Data;
using GARCA.Utils.Logging;

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
            using (var connection = DBContext.OpenConnection())
            {
                await connection.ExecuteAsync("SplitsPostSave", new {Sid = obj.Id, Tid = obj.TransactionsId }, commandType: CommandType.StoredProcedure);
            }
        }       

        public async Task<IEnumerable<Splits>?> GetbyTransactionidNull()
        {
            using (var connection = DBContext.OpenConnection())
            {
                return await connection.SelectAsync<Splits>(x => x.TransactionsId == null);
            }
        }

        public async Task<IEnumerable<Splits>?> GetbyTransactionid(int transactionid)
        {
            using (var connection = DBContext.OpenConnection())
            {
                return await connection.SelectAsync<Splits>(x => x.TransactionsId == transactionid);
            }
        }              
    }
}
