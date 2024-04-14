using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class SplitsValidations : AbstractValidator<Splits>
    {
        public SplitsValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.TransactionsId).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.CategoriesId).GreaterThanOrEqualTo(-2).NotEmpty();
            RuleFor(model => model.AmountIn).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.AmountOut).GreaterThanOrEqualTo(0).NotEmpty();
            RuleFor(model => model.TagsId).GreaterThanOrEqualTo(0);
            RuleFor(model => model.TranferId).GreaterThanOrEqualTo(0);
        }
    }
}
 