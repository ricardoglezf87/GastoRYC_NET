 using GARCA.Models;
using GARCA.wsData.Repositories;

namespace GARCA.Data.Services
{
    public class AccountsTypesService : ServiceBase<AccountsTypesRepository,AccountsTypes>
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
    }
}
