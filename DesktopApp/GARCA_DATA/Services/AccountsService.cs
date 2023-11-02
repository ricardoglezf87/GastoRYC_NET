using GARCA.Data.Managers;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
{
    public class AccountsService
    {
        protected readonly AccountsManager accountsManager;

        public AccountsService()
        {
            accountsManager = new AccountsManager();
        }

        public HashSet<Accounts?>? GetAll()
        {
            return accountsManager.GetAll()?.ToHashSet();
        }

        public HashSet<Accounts?>? GetAllOpened()
        {
            return accountsManager.GetAllOpened()?.ToHashSet();
        }

        public async Task<HashSet<Accounts?>?> GetAllOpenedAync()
        {
            return await Task.Run(() => GetAllOpened());
        }

        public Accounts? GetById(int? id)
        {
            return accountsManager.GetById(id);
        }

        public Accounts? GetByCategoryId(int? id)
        {
            return accountsManager.GetByCategoryId(id);
        }

        public void Update(Accounts accounts)
        {
            accountsManager.Update(accounts);
        }

        public void Delete(Accounts accounts)
        {
            accountsManager.Delete(accounts);
        }

        public Decimal GetBalanceByAccount(int? id)
        {
            return iTransactionsService.GetByAccount(id)?.Sum(x => x.Amount) ?? 0;
        }
    }
}
