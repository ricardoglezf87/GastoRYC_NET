using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using GARCA_DATA.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

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
                Id = int.MaxValue,
                Date = DateTime.Now,
                PeriodsRemindersId = periodid,
                AccountsId = accountid,
                PersonsId = perssonid,
                CategoriesId = categoryid,
                AmountIn = decimal.MaxValue,
                AmountOut = decimal.MaxValue,
                TransactionsStatusId = statustid,
            };
        }
    }
}