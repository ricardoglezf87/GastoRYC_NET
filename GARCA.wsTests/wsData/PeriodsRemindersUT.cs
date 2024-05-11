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
    public class PeriodsRemindersUT : BaseUT<PeriodsReminders,PeriodsRemindersValidations,PeriodsRemindersRepository>
    {
        public override PeriodsReminders MakeChange(PeriodsReminders obj)
        {
            obj.Description = getNextWord();
            return obj;
        }

        public override PeriodsReminders CreateObj()
        {
            return new PeriodsReminders()
            {
                Id = 0,
                Description = getNextWord()
            };
        }
    }
}