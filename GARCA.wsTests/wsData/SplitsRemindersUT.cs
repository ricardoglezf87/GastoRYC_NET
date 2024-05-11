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
    public class SplitsRemindersUT : BaseUT<SplitsReminders, SplitsRemindersValidations, SplitsRemindersRepository>
    {
        public override SplitsReminders MakeChange(SplitsReminders obj)
        {
            obj.Memo = "TestDescripUpdate";
            return obj;
        }

        public override SplitsReminders CreateObj()
        {
            var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
            var trans = new TransactionsRemindersRepository().Save(new TransactionsRemindersUT().CreateObj()).Result;

            return new SplitsReminders()
            {
                Id = 0,
                CategoriesId = category.Id,
                TransactionsId = trans.Id,
                AmountIn = getNextDecimal(),
                AmountOut = getNextDecimal()
            };
        }
    }
}