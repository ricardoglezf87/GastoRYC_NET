using BBDDLib.Models;
using System.Collections.Generic;

namespace GastosRYC.BBDDLib.Services
{
    public interface IAccountsTypesService
    {
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

        public List<AccountsTypes>? getAll();

        public AccountsTypes? getByID(int? id);

        public bool accountExpensives(int? types);

    }
}
