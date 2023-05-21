using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class AccountsTypesService
    {
        private readonly AccountsTypesManager accountsTypesManager;
        private static AccountsTypesService? _instance;
        private static readonly object _lock = new object();

        public static AccountsTypesService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new AccountsTypesService();
                    }
                }
                return _instance;
            }
        }

        private AccountsTypesService()
        {
            accountsTypesManager = new();
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

        public List<AccountsTypes?>? getAll()
        {
            return accountsTypesManager.getAll()?.toListBO();
        }

        public AccountsTypes? getByID(int? id)
        {
            return (AccountsTypes?)accountsTypesManager.getByID(id);
        }

        public bool accountExpensives(int? types)
        {
            return types is ((int)eAccountsTypes.Cash) or ((int)eAccountsTypes.Banks) or ((int)eAccountsTypes.Cards);
        }

    }
}
