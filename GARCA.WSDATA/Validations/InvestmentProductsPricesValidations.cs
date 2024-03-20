using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class InvestmentProductsPricesValidations : AbstractValidator<InvestmentProductsPrices>
    {
        public InvestmentProductsPricesValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Date).NotEmpty();
            RuleFor(model => model.InvestmentProductsid).GreaterThan(0).NotEmpty();
            RuleFor(model => model.Prices).GreaterThan(0).NotEmpty();
        }
    }
}
 