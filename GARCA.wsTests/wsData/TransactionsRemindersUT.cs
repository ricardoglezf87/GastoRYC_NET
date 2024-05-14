using GARCA.Models;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;

using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class TransactionsRemindersUT : BaseUT<TransactionsReminders,TransactionsRemindersValidations,TransactionsRemindersRepository>
    {
        public override TransactionsReminders MakeChange(TransactionsReminders obj)
        {
            obj.Memo = getNextWord();
            return obj;
        }

        public override TransactionsReminders CreateObj()
        {
            var period = new PeriodsRemindersRepository().Save(new PeriodsRemindersUT().CreateObj()).Result;
            var person = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
            var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
            var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
            var statust = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;

            return new TransactionsReminders()
            {
                Id = 0,
                Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                PeriodsRemindersId = period.Id,
                AccountsId = account.Id,
                PersonsId = person.Id,
                CategoriesId = category.Id,
                AmountIn = getNextDecimal(),
                AmountOut = getNextDecimal(),
                TransactionsStatusId = statust.Id,
            };
        }
    }
}