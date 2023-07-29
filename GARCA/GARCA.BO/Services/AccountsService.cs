using GARCA.BO.Extensions;
using GARCA.BO.Models;
using GARCA.BO.ModelsView;
using GARCA.DAO.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GARCA.BO.Services
{
    public class AccountsService
    {
        private readonly AccountsManager accountsManager;
        private static AccountsService? _instance;
        private static readonly object _lock = new();

        public static AccountsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new AccountsService();
                    }
                }
                return _instance;
            }
        }

        private AccountsService()
        {
            accountsManager = new();
        }

        public HashSet<Accounts?>? getAll()
        {
            return accountsManager.getAll()?.toHashSetBO();
        }

        public List<Accounts?>? getAllOpened()
        {
            return accountsManager.getAllOpened()?.toListBO();
        }

        public async Task<List<Accounts?>?> getAllOpenedAync()
        {
            return await Task.Run(() => getAllOpened());
        }

        public List<AccountsView>? getAllOpenedListView()
        {
            return accountsManager.getAllOpened()?.toListViewBO();
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
            return TransactionsService.Instance.getByAccount(id)?.Sum(x => x.amount) ?? 0;
        }
    }
}
