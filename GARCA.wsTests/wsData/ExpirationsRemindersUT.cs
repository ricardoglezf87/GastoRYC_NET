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
    public class ExpirationsRemindersUT : BaseUT<ExpirationsReminders,ExpirationsRemindersValidations,ExpirationsRemindersRepository>
    {
        public override ExpirationsReminders MakeChange(ExpirationsReminders obj)
        {
            obj.Done = true;
            return obj;
        }

        public override ExpirationsReminders CreateObj()
        {
            var transid = new TransactionsRemindersRepository().Save(new TransactionsRemindersUT().CreateObj()).Result;

            return new ExpirationsReminders()
            {
                Id = 0,
                Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                TransactionsRemindersId = transid.Id,
            };
        }
    }
}