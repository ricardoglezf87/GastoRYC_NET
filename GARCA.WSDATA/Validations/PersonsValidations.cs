using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class PersonsValidations : AbstractValidator<Persons>
    {
        public PersonsValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Categoryid).GreaterThanOrEqualTo(0).NotEmpty();
        }
    }
}
 