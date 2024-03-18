using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class InvestmentProductsValidations : AbstractValidator<InvestmentProducts>
    {
        public InvestmentProductsValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Description).NotEmpty();
            RuleFor(model => model.InvestmentProductsTypesId).GreaterThanOrEqualTo(0);
        }
    }
}
 