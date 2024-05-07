using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class AccountsTypesUT : BaseUT<AccountsTypes,AccountsTypesValidations,AccountsTypesRepository>
    {
        public override AccountsTypes MakeChange(AccountsTypes obj)
        {
            obj.Description = "TestDescripUpdate";
            return obj;
        }

        public override AccountsTypes CreateObj()
        {
            return new AccountsTypes()
            {
                Id = 0,
                Description = "TestDescrip"
            };
        }
    }
}