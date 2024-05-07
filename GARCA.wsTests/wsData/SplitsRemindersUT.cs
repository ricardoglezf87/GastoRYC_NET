using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using Microsoft.AspNetCore.Http.HttpResults;

using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class SplitsRemindersUT : BaseUT<SplitsReminders,SplitsRemindersValidations>
    {
        public override SplitsReminders MakeChange(SplitsReminders obj)
        {
            obj.Memo = "TestDescripUpdate";
            return obj;
        }

        public override SplitsReminders CreateObj()
        {
            var categoryid = new CategoriesRepository().Insert(new CategoriesUT().CreateObj()).Result;
            var transid = new TransactionsRemindersRepository().Insert(new TransactionsRemindersUT().CreateObj()).Result;

            return new SplitsReminders()
            {
                Id = 0,
                CategoriesId = categoryid,
                TransactionsId = transid,
                AmountIn = getNextDecimal(),
                AmountOut = getNextDecimal()
            };
        }
    }
}