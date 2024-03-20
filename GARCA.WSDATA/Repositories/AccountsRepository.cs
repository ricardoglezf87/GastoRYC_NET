using Dommel;
using GARCA.wsData.Repositories;
using GARCA.Models;


namespace GARCA_DATA.Repositories
{
    public class AccountsRepository : RepositoryBase<Accounts>
    {
        public override async Task<IEnumerable<Accounts>?> GetAll()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.GetAllAsync<Accounts, AccountsTypes, Accounts>();
            }
        }
    }
}
