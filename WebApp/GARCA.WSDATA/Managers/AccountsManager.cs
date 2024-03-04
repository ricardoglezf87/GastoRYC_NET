using Dommel;
using GARCA.wsData.Managers;
using GARCA.Models;


namespace GARCA_DATA.Managers
{
    public class AccountsManager : ManagerBase<Accounts>
    {
        public override async Task<IEnumerable<Accounts>?> GetAll()
        {
            return await dbContext.OpenConnection().GetAllAsync<Accounts, AccountsTypes, Accounts>();
        }
    }
}
