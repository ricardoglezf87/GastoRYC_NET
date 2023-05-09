using BBDDLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace GastosRYC.BBDDLib.Services
{
    public class AccountsTypesService
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

        public List<AccountsTypes>? getAll()
        {
            return RYCContextService.getInstance().BBDD.accountsTypes?.ToList();
        }

        public AccountsTypes? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.accountsTypes?.FirstOrDefault(x => id.Equals(x.id));
        }

        public bool accountExpensives(int? types)
        {
            return (types == (int)eAccountsTypes.Cash || types == (int)eAccountsTypes.Banks || types == (int)eAccountsTypes.Cards);
        }

    }
}
