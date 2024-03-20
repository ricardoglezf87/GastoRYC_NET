using Dommel;
using GARCA.wsData.Repositories;
using GARCA.Models;


namespace GARCA_DATA.Repositories
{
    public class AccountsRepository : RepositoryBase<Accounts>
    {
        public override async Task<IEnumerable<Accounts>?> GetAll()
        {
            return await dbContext.OpenConnection().GetAllAsync<Accounts, AccountsTypes, Accounts>();
        }
    }
}
