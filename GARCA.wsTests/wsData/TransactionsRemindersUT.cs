using GARCA.Models;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;

using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class TransactionsRemindersUT : BaseUT<TransactionsReminders,TransactionsRemindersValidations>
    {
        public override TransactionsReminders MakeChange(TransactionsReminders obj)
        {
            obj.Memo = "TestDescripUpdate";
            return obj;
        }

        public override TransactionsReminders CreateObj()
        {
            var periodid = new PeriodsRemindersRepository().Insert(new PeriodsReminderUT().CreateObj()).Result;
            var perssonid = new PersonsRepository().Insert(new PersonsUT().CreateObj()).Result;
            var categoryid = new CategoriesRepository().Insert(new CategoriesUT().CreateObj()).Result;
            var accountid = new AccountsRepository().Insert(new AccountsUT().CreateObj()).Result;
            var statustid = new TransactionsStatusRepository().Insert(new TransactionsStatusUT().CreateObj()).Result;

            return new TransactionsReminders()
            {
                Id = 0,
                Date = DateTime.Now,
                PeriodsRemindersId = periodid,
                AccountsId = accountid,
                PersonsId = perssonid,
                CategoriesId = categoryid,
                AmountIn = new decimal(9999.9999),
                AmountOut = new decimal(9999.9999),
                TransactionsStatusId = statustid,
            };
        }
    }
}