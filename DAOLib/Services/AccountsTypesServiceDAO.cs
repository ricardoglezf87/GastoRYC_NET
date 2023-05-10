using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class AccountsTypesServiceDAO
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

        public List<AccountsTypesDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.accountsTypes?.ToList();
        }

        public AccountsTypesDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.accountsTypes?.FirstOrDefault(x => id.Equals(x.id));
        }

        public bool accountExpensives(int? types)
        {
            return (types == (int)eAccountsTypes.Cash || types == (int)eAccountsTypes.Banks || types == (int)eAccountsTypes.Cards);
        }

    }
}
