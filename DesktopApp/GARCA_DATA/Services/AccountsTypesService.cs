using Dapper;
using GARCA.Data.Managers;
using GARCA.Models;
using GARCA_DATA.Services;
using static GARCA.Data.IOC.DependencyConfig;
using Microsoft.Data.Sqlite;

namespace GARCA.Data.Services
{
    public class AccountsTypesService : IServiceCache<AccountsTypes>
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

        protected override IEnumerable<AccountsTypes>? GetAllCache()
        {
            return iRycContextService.getConnection().Query<AccountsTypes>(
                @"
                    select * 
                    from AccountsTypes
                ").AsEnumerable();

        }

        public bool AccountExpensives(int? types)
        {
            return types is (int)EAccountsTypes.Cash or (int)EAccountsTypes.Banks or (int)EAccountsTypes.Cards;
        }

    }
}
