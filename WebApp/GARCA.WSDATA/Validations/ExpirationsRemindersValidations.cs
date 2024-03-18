using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class ExpirationsRemindersValidations : AbstractValidator<ExpirationsReminders>
    {
        public ExpirationsRemindersValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Date).NotEmpty();
            RuleFor(model => model.TransactionsRemindersId).GreaterThanOrEqualTo(0);
        }
    }
}
 