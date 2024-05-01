using Dapper;
using Dommel;
using GARCA.Data.Managers;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA_DATA.Managers
{
    public class AccountsManager : ManagerBase<Accounts>
    {
        public async Task<IEnumerable<Accounts>?> GetAllOpened()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<Accounts, AccountsTypes, Accounts>(x => x.Closed == null || x.Closed == false);
            }
        }

        public async Task<Accounts?> GetByCategoryId(int id)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.FirstOrDefaultAsync<Accounts, AccountsTypes, Accounts>(x => x.Categoryid == id);
            }
        }

        public override async Task<IEnumerable<Accounts>?> GetAll()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.GetAllAsync<Accounts, AccountsTypes, Accounts>();
            }
        }
    }
}
