using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class PeriodsRemindersValidations : AbstractValidator<PeriodsReminders>
    {
        public PeriodsRemindersValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Description).NotEmpty();
        }
    }
}
 