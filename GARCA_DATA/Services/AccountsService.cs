using GARCA.Models;
using GARCA_DATA.Managers;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class AccountsService : ServiceBase<AccountsManager, Accounts>
    {
        public async Task<IEnumerable<Accounts>?> GetAllOpened()
        {
            return (await GetAll())?.Where(x => x.Closed is null or false).ToHashSet();
        }

        public async Task<Accounts?> GetByCategoryId(int id)
        {
            return (await GetAll())?.First(x => x.Categoryid == id);
        }

        public async Task<Decimal> GetBalanceByAccount(int? id)
        {
            return (await iTransactionsService.GetByAccount(id))?.Sum(x => x.Amount) ?? 0;
        }
    }
}
