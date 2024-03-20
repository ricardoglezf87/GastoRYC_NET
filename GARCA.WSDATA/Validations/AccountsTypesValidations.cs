using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class CategoriesTypesValidations: AbstractValidator<CategoriesTypes>
    {
        public CategoriesTypesValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Description).NotEmpty();
        }
    }
}
 