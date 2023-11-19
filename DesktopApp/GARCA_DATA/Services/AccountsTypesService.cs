using Dapper;
using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Data.Services;
using static GARCA.Data.IOC.DependencyConfig;
using GARCA_DATA.Managers;

namespace GARCA.Data.Services
{
    public class AccountsTypesService : ServiceBase<AccountsTypesManager, AccountsTypes>
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
