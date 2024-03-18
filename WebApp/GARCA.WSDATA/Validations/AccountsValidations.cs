using FluentValidation;
using GARCA.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace GARCA.wsData.Validations
{
    public class AccountsValidations: AbstractValidator<Accounts>
    {
        public AccountsValidations()
        {
            RuleFor(model => model.Id).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Description).NotEmpty();
            RuleFor(model => model.AccountsTypesId).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Categoryid).GreaterThanOrEqualTo(0);
        }
    }
}
 