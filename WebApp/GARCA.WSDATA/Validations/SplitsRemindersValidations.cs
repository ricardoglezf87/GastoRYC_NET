using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class SplitsRemindersValidations : AbstractValidator<SplitsReminders>
    {
        public SplitsRemindersValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.TransactionsId).GreaterThanOrEqualTo(0);
            RuleFor(model => model.CategoriesId).GreaterThanOrEqualTo(0);
            RuleFor(model => model.AmountIn).GreaterThanOrEqualTo(0);
            RuleFor(model => model.AmountOut).GreaterThanOrEqualTo(0);
        }
    }
}
 