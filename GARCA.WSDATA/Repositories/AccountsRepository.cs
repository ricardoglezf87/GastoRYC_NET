using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class AccountsRepository : RepositoryBase<Accounts>
    {
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
