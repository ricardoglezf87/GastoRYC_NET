using GARCA.Models;
using GARCA_DATA.Managers;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class AccountsService : ServiceBase<AccountsManager, Accounts>
    {
        public async Task<IEnumerable<Accounts>?> GetAllOpened()
        {
            return await manager.GetAllOpened();
        }

        public async Task<Accounts?> GetByCategoryId(int id)
        {
            return await manager.GetByCategoryId(id);
        }

        public async Task<Decimal> GetBalanceByAccount(int? id)
        {
            return (await iTransactionsService.GetByAccount(id))?.Sum(x => x.Amount) ?? 0;
        }
    }
}
