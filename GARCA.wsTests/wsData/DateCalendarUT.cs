using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using Microsoft.AspNetCore.Http.HttpResults;

using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class DateCalendarUT : BaseUT<DateCalendar,DateCalendarValidations,DateCalendarRepository>
    {
        public override DateCalendar MakeChange(DateCalendar obj)
        {
            obj.Year = DateTime.Now.Year + 1;
            return obj;
        }

        public override DateCalendar CreateObj()
        {
            return new DateCalendar()
            {
                Id = 0,
                Date = DateTime.Now,
                Day = DateTime.Now.Day,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
            };
        }
    }
}