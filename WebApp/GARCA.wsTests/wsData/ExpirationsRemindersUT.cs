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
    public class ExpirationsRemindersUT : BaseUT<ExpirationsReminders,ExpirationsRemindersValidations>
    {
        public override ExpirationsReminders CreateObj()
        {
       
            return new ExpirationsReminders()
            {
                Id = int.MaxValue,
                Date = DateTime.Now,
                TransactionsRemindersId = int.MaxValue,
            };
        }
    }
}