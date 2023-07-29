using GARCA.Utlis.Extensions;
using GARCA.BO.Models;
using GARCA.View.ViewModels;
using GARCA.DAO.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GARCA.Utils.IOC;

namespace GARCA.BO.Services
{
    public class AccountsService
    {
        private readonly AccountsManager accountsManager;
        
        public AccountsService()
        {
            accountsManager = new AccountsManager();
        }

        public HashSet<Accounts?>? getAll()
        {
            return accountsManager.getAll()?.toHashSetBO();
        }

        public HashSet<Accounts?>? getAllOpened()
        {
            return accountsManager.getAllOpened()?.toHashSetBO();
        }

        public async Task<HashSet<Accounts?>?> getAllOpenedAync()
        {
            return await Task.Run(() => getAllOpened());
        }

        public HashSet<AccountsView>? getAllOpenedListView()
        {
            return accountsManager.getAllOpened()?.toHashSetViewBO();
        }

        public Accounts? getByID(int? id)
        {
            return (Accounts?)accountsManager.getByID(id);
        }

        public Accounts? getByCategoryId(int? id)
        {
            return (Accounts?)accountsManager.getByCategoryId(id);
        }

        public void update(Accounts accounts)
        {
            accountsManager.update(accounts?.toDAO());
        }

        public void delete(Accounts accounts)
        {
            accountsManager.delete(accounts?.toDAO());
        }

        public Decimal getBalanceByAccount(int? id)
        {
            return DependencyConfig.iTransactionsService.getByAccount(id)?.Sum(x => x.amount) ?? 0;
        }
    }
}
