using GARCA.Data.Managers;
using GARCA.Models;

namespace GARCA.Data.Services
{
    public class AccountsTypesService : ServiceBaseWS<AccountsTypes>
    {
        public enum EAccountsTypes
        {
            Cash = 1,
            Banks = 2,
            Cards = 3,
            Invests = 4,
            Loans = 5,
            Bounds = 6,
            Savings = 7
        }

        public bool AccountExpensives(int? types)
        {
            return types is (int)EAccountsTypes.Cash or (int)EAccountsTypes.Banks or (int)EAccountsTypes.Cards;
        }

    }
}
