using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using Microsoft.AspNetCore.Http.HttpResults;

using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class InvestmentProductsTypesUT : BaseUT<InvestmentProductsTypes, InvestmentProductsTypesValidations>
    {
        public override InvestmentProductsTypes MakeChange(InvestmentProductsTypes obj)
        {
            obj.Description = "TestDescripUpdate";
            return obj;
        }

        public override InvestmentProductsTypes CreateObj()
        {
            return new InvestmentProductsTypes()
            {
                Id = 0,
                Description = "TestDescrip"
            };
        }
    }
}