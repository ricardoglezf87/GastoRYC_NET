using GARCA.Models;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using GARCA_DATA.Repositories;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class AccountsUT : BaseUT<Accounts,AccountsValidations>
    {
        public override Accounts CreateObj()
        {
            var accountsTypesId = new AccountsTypesRepository().Insert(new AccountsTypesUT().CreateObj()).Result;
            var categoryid = new CategoriesRepository().Insert(new CategoriesUT().CreateObj()).Result;

            return new Accounts()
            {
                Id = int.MaxValue,
                Description = "TestDescrip",
                AccountsTypesId = accountsTypesId,
                Categoryid = categoryid,
            };
        }
    }
}