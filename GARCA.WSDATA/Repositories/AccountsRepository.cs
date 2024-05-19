using Dommel;
using GARCA.Models;
using GARCA.Utils.Extensions;


namespace GARCA.wsData.Repositories
{
    public class AccountsRepository : RepositoryBase<Accounts>
    {
        public override async Task<Accounts> Save(Accounts obj)
        {
            obj = await base.Save(obj);
            return await GetById(obj.Id) ?? obj;
        }

        public async Task<IEnumerable<Accounts>?> GetAllOpened()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.SelectAsync<Accounts, AccountsTypes, Accounts>(x => x.Closed == null || x.Closed == false);
            }
        }

        public async Task<Accounts?> GetByCategoryId(int id)
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.FirstOrDefaultAsync<Accounts, AccountsTypes, Accounts>(x => x.Categoryid == id);
            }
        }

        public override async Task<IEnumerable<Accounts>?> GetAll()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.GetAllAsync<Accounts, AccountsTypes, Accounts>();
            }
        }
    }
}
