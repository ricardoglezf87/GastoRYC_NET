using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class DateCalendarValidations : AbstractValidator<DateCalendar>
    {
        public DateCalendarValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Date).NotEmpty();
            RuleFor(model => model.Day).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Month).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Year).GreaterThanOrEqualTo(0);
        }
    }
}
 