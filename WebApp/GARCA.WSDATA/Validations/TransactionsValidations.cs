using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class TransactionsValidations : AbstractValidator<Transactions>
    {
        public TransactionsValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Date).NotEmpty();
            RuleFor(model => model.AccountsId).GreaterThanOrEqualTo(0);
            RuleFor(model => model.CategoriesId).GreaterThanOrEqualTo(0);
            RuleFor(model => model.AmountIn).GreaterThanOrEqualTo(0);
            RuleFor(model => model.AmountOut).GreaterThanOrEqualTo(0);
            RuleFor(model => model.TransactionsStatusId).GreaterThanOrEqualTo(0);
        }
    }
}
 