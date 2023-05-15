using DAOLib.Models;

namespace DAOLib.Managers
{
    public class AccountsTypesManager : ManagerBase<AccountsTypesDAO>
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
            return (types is ((int)eAccountsTypes.Cash) or ((int)eAccountsTypes.Banks) or ((int)eAccountsTypes.Cards));
        }

    }
}
