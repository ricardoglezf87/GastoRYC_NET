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
            RuleFor(model => model.Day).GreaterThanOrEqualTo(1).LessThanOrEqualTo(31).NotEmpty();
            RuleFor(model => model.Month).GreaterThanOrEqualTo(1).LessThanOrEqualTo(12).NotEmpty();
            RuleFor(model => model.Year).GreaterThanOrEqualTo(1980).LessThanOrEqualTo(2100).NotEmpty();
        }
    }
}
 