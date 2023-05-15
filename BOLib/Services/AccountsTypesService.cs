using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class AccountsTypesService
    {
        private readonly AccountsTypesManager accountsTypesManager;

        public AccountsTypesService()
        {
            accountsTypesManager = InstanceBase<AccountsTypesManager>.Instance;
        }

        public enum eAccountsTypes : int
        {
            Cash = 1,
            Banks = 2,
            Cards = 3,
            Invests = 4,
            Loans = 5,
            bounds = 6,
            Savings = 7
        }

        public List<AccountsTypes>? getAll()
        {
            return accountsTypesManager.getAll()?.toListBO();
        }

        public AccountsTypes? getByID(int? id)
        {
            return (AccountsTypes)accountsTypesManager.getByID(id);
        }

        public bool accountExpensives(int? types)
        {
            return (types == (int)eAccountsTypes.Cash || types == (int)eAccountsTypes.Banks || types == (int)eAccountsTypes.Cards);
        }

    }
}
