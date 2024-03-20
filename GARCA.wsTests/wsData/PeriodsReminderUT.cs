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
    public class PeriodsReminderUT : BaseUT<PeriodsReminders,PeriodsRemindersValidations>
    {
        public override PeriodsReminders MakeChange(PeriodsReminders obj)
        {
            obj.Description = "TestDescripUpdate";
            return obj;
        }

        public override PeriodsReminders CreateObj()
        {
            return new PeriodsReminders()
            {
                Id = int.MaxValue,
                Description = "TestDescrip"
            };
        }
    }
}