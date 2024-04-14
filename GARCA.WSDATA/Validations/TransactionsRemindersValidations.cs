using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class TransactionsRemindersValidations : AbstractValidator<TransactionsReminders>
    {
        public TransactionsRemindersValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Date).NotEmpty();
            RuleFor(model => model.PeriodsRemindersId).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.AccountsId).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.CategoriesId).GreaterThanOrEqualTo(-2).NotEmpty();
            RuleFor(model => model.AmountIn).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.AmountOut).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.TransactionsStatusId).GreaterThanOrEqualTo(0).NotEmpty();
        }
    }
}
 