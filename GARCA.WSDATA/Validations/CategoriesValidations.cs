using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class CategoriesValidations: AbstractValidator<Categories>
    {
        public CategoriesValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Description).NotEmpty();
            RuleFor(model => model.CategoriesTypesId).GreaterThanOrEqualTo(0).NotEmpty();
        }
    }
}
 