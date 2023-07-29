using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class AccountsTypesService
    {
        private readonly AccountsTypesManager accountsTypesManager;
        private static AccountsTypesService? _instance;
        private static readonly object _lock = new();

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

        public HashSet<AccountsTypes?>? getAll()
        {
            return accountsTypesManager.getAll()?.toHashSetBO();
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
