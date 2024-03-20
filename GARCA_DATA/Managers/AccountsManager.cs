using Dommel;
using GARCA.Data.Managers;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA_DATA.Managers
{
    public class AccountsManager : ManagerBase<Accounts>
    {
        public override async Task<IEnumerable<Accounts>?> GetAll()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.GetAllAsync<Accounts, AccountsTypes, Accounts>();
            }
        }
    }
}
