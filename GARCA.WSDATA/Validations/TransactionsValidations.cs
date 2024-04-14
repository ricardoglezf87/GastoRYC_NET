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
            RuleFor(model => model.AccountsId).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.PersonsId).GreaterThanOrEqualTo(0);
            RuleFor(model => model.CategoriesId).GreaterThanOrEqualTo(-2).NotEmpty();
            RuleFor(model => model.AmountIn).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.AmountOut).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.TransactionsStatusId).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.InvestmentProductsId).GreaterThanOrEqualTo(0);
            RuleFor(model => model.TagsId).GreaterThanOrEqualTo(0);
            RuleFor(model => model.TranferId).GreaterThanOrEqualTo(0);
            RuleFor(model => model.TranferSplitId).GreaterThanOrEqualTo(0);
        }
    }
}
 