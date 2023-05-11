using AutoMapper;
using BOLib.Extensions;
using BOLib.Helpers;
using BOLib.Models;
using BOLib.ModelsView;
using DAOLib.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BOLib.Services
{
    public class AccountsService
    {
        private readonly AccountsManager accountsManager;

        public AccountsService()
        {
            accountsManager = new();
        }

        public List<Accounts>? getAll()
        {
            return accountsManager.getAll()?.toListBO();
        }

        public List<Accounts>? getAllOrderByAccountsTypesId()
        {
            return accountsManager.getAllOrderByAccountsTypesId()?.toListBO();
        }

        public List<Accounts>? getAllOpened()
        {
            return accountsManager.getAllOpened()?.toListBO();
        }

        public List<AccountsView>? getAllOpenedListView()
        {
            return accountsManager.getAllOpened()?.toListViewBO();
        }

        public Accounts? getByID(int? id)
        {
            return (Accounts) accountsManager.getByID(id);
        }

        public void update(Accounts accounts)
        {
            accountsManager.update(accounts.toDAO());
        }

        public void delete(Accounts accounts)
        {
            accountsManager.delete(accounts.toDAO());
        }
    }
}
