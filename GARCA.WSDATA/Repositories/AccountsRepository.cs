using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
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
