using GARCA.Models;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;

using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class TransactionsUT : BaseUT<Transactions,TransactionsValidations,TransactionsRepository>
    {
        public override Transactions MakeChange(Transactions obj)
        {
            obj.Memo = "TestDescripUpdate";
            return obj;
        }

        public override Transactions CreateObj()
        {
            var perssonid = new PersonsRepository().Insert(new PersonsUT().CreateObj()).Result;
            var categoryid = new CategoriesRepository().Insert(new CategoriesUT().CreateObj()).Result;
            var accountid = new AccountsRepository().Insert(new AccountsUT().CreateObj()).Result;
            var statustid = new TransactionsStatusRepository().Insert(new TransactionsStatusUT().CreateObj()).Result;

            return new Transactions()
            {
                Id = 0,
                Date = DateTime.Now,
                AccountsId = accountid,
                PersonsId = perssonid,
                CategoriesId = categoryid,
                AmountIn = getNextDecimal(),
                AmountOut = getNextDecimal(),
                TransactionsStatusId = statustid,
            };
        }
    }
}