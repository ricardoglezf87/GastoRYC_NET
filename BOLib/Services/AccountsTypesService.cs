using BOLib.Helpers;
using BOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
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
            return MapperConfig.InitializeAutomapper().Map<List<AccountsTypes>>(RYCContextService.getInstance().BBDD.accountsTypes?.ToList());
        }

        public AccountsTypes? getByID(int? id)
        {
            return MapperConfig.InitializeAutomapper().Map<AccountsTypes>(RYCContextService.getInstance().BBDD.accountsTypes?.FirstOrDefault(x => id.Equals(x.id)));
        }

        public bool accountExpensives(int? types)
        {
            return (types == (int)eAccountsTypes.Cash || types == (int)eAccountsTypes.Banks || types == (int)eAccountsTypes.Cards);
        }

    }
}
