using GARCA.DAO.Models;

namespace GARCA.DAO.Managers
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

    }
}
