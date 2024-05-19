using GARCA.Models;
using GARCA.wsData.Repositories;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class AccountsService : ServiceBase<AccountsRepository, Accounts>
    {
        public async Task<IEnumerable<Accounts>?> GetAllOpened()
        {
            return await repository.GetAllOpened();
        }

        public async Task<Decimal> GetBalanceByAccount(int? id)
        {
            return (await iTransactionsService.GetByAccount(id))?.Sum(x => x.Amount) ?? 0;
        }
    }
}
