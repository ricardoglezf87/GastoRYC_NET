using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class BaseValidations: AbstractValidator<ModelBase>
    {
        public BaseValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
        }
    }
}
 