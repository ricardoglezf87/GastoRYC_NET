using AutoMapper;
using BOLib.Extensions;
using BOLib.Helpers;
using BOLib.Models;
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
            return accountsManager.getAll()?.toListAccounts();
        }

        public List<Accounts>? getAllOrderByAccountsTypesId()
        {
            return accountsManager.getAllOrderByAccountsTypesId()?.toListAccounts();
        }

        public List<Accounts>? getAllOpened()
        {
            return accountsManager.getAllOpened()?.toListAccounts();
        }

        public List<Accounts>? getAllOpenedOrderbyAccountTypeId()
        {
            return (List<Accounts>?) (getAllOpened()?.OrderBy(x=>x.accountsTypesid))?.ToList();
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
