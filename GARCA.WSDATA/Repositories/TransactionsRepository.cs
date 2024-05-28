using Dapper;
using Dommel;
using GARCA.Models;
using GARCA.Utils.Extensions;
using System.Data;
using static GARCA.Utils.Enums.EnumComun;


namespace GARCA.wsData.Repositories
{
    public class TransactionsRepository : RepositoryBase<Transactions>
    {
        public override async Task<IEnumerable<Transactions>?> GetAll()
        {
            using (var connection = DBContext.OpenConnection())
            {
                return await connection.GetAllAsync<Transactions, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, Transactions>();
            }
        }

        public override async Task<Transactions> Save(Transactions obj)
        {
            obj.Date = obj.Date.RemoveTime();
            obj = await base.Save(obj);
            await postChange(obj);
            _ = UpdateDefaultPerson(obj.PersonsId ?? -99);
            return await GetById(obj.Id) ?? obj;
        }

        public override async Task<bool> Delete(Transactions obj)
        {
            bool result = await base.Delete(obj);
            await postChange(obj);
            _ = UpdateDefaultPerson(obj.PersonsId ?? -99);
            return result;
        }

        private async Task postChange(Transactions obj)
        {
            using (var connection = DBContext.OpenConnection())
            {                
                await connection.ExecuteAsync("TransactionPostSave", new { Tid = obj.Id, TaccountId = obj.AccountsId, Tdate = obj.Date }, commandType: CommandType.StoredProcedure);            
            }
        }

        public async Task<IEnumerable<Transactions>?> GetByAccount(int accountId)
        {
            using (var connection = DBContext.OpenConnection())
            {
                return await connection.SelectAsync<Transactions, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, Transactions>
                    (x => x.AccountsId == accountId);
            }
        }

        public async Task UpdateDefaultPerson(int personId)
        {
            using (var connection = DBContext.OpenConnection())
            {
                await connection.ExecuteAsync("UpdatePersonsCategoriesId", new { person_id = personId }, commandType: CommandType.StoredProcedure);
                connection.Close();
            }
        }        
    }
}
