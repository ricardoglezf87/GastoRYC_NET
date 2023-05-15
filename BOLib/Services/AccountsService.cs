using BOLib.Extensions;
using BOLib.Models;
using BOLib.ModelsView;
using DAOLib.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class AccountsService
    {
        private readonly AccountsManager accountsManager;
        private readonly TransactionsService transactionsService;

        public AccountsService()
        {
            accountsManager = InstanceBase<AccountsManager>.Instance;
            transactionsService = InstanceBase<TransactionsService>.Instance;
        }

        public List<Accounts?>? getAll()
        {
            return accountsManager.getAll()?.toListBO();
        }

        public List<Accounts?>? getAllOrderByAccountsTypesId()
        {
            return accountsManager.getAllOrderByAccountsTypesId()?.toListBO();
        }

        public List<Accounts?>? getAllOpened()
        {
            return accountsManager.getAllOpened()?.toListBO();
        }

        public List<AccountsView>? getAllOpenedListView()
        {
            return accountsManager.getAllOpened()?.toListViewBO();
        }

        public Accounts? getByID(int? id)
        {
            return (Accounts?)accountsManager.getByID(id);
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
            return transactionsService.getByAccount(id)?.Sum(x => x.amount) ?? 0;
        }

        public Decimal getBalanceByAccount(Accounts? accounts)
        {
            return getBalanceByAccount(accounts.id);
        }
    }
}
