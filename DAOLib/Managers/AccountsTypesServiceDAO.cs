using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class AccountsTypesServiceDAO : IServiceDAO<AccountsTypesDAO>
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

        public bool accountExpensives(int? types)
        {
            return (types == (int)eAccountsTypes.Cash || types == (int)eAccountsTypes.Banks || types == (int)eAccountsTypes.Cards);
        }

    }
}
