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
    public class DateCalendarUT : BaseUT<DateCalendar,DateCalendarValidations>
    {
        public override DateCalendar CreateObj()
        {
            return new DateCalendar()
            {
                Id = int.MaxValue,
                Date = DateTime.Now,
                Day = DateTime.Now.Day,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
            };
        }
    }
}