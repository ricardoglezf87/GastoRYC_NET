using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utils.IOC;
using GARCA.Utlis.Extensions;
using GARCA.View.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GARCA.BO.Services
{
    public class AccountsService
    {
        private readonly AccountsManager accountsManager;

        public AccountsService()
        {
            accountsManager = new AccountsManager();
        }

        public HashSet<Accounts?>? GetAll()
        {
            return accountsManager.GetAll()?.ToHashSetBo();
        }

        public HashSet<Accounts?>? GetAllOpened()
        {
            return accountsManager.GetAllOpened()?.ToHashSetBo();
        }

        public async Task<HashSet<Accounts?>?> GetAllOpenedAync()
        {
            return await Task.Run(() => GetAllOpened());
        }

        public HashSet<AccountsView>? GetAllOpenedListView()
        {
            return accountsManager.GetAllOpened()?.ToHashSetViewBo();
        }

        public Accounts? GetById(int? id)
        {
            return (Accounts?)accountsManager.GetById(id);
        }

        public Accounts? GetByCategoryId(int? id)
        {
            return (Accounts?)accountsManager.GetByCategoryId(id);
        }

        public void Update(Accounts accounts)
        {
            accountsManager.Update(accounts.ToDao());
        }

        public void Delete(Accounts accounts)
        {
            accountsManager.Delete(accounts.ToDao());
        }

        public Decimal GetBalanceByAccount(int? id)
        {
            return DependencyConfig.ITransactionsService.GetByAccount(id)?.Sum(x => x.Amount) ?? 0;
        }
    }
}
