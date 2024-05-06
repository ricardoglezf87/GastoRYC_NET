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
    public class InvestmentProductsUT : BaseUT<InvestmentProducts, InvestmentProductsValidations>
    {
        public override InvestmentProducts MakeChange(InvestmentProducts obj)
        {
            obj.Description = "TestDescripUpdate";
            return obj;
        }

        public override InvestmentProducts CreateObj()
        {
            var investmentProductsTypesId = new InvestmentProductsTypesRepository().
                    Insert(new InvestmentProductsTypesUT().CreateObj()).Result;

            return new InvestmentProducts()
            {
                Id = 0,
                Description = "TestDescrip",
                InvestmentProductsTypesId = investmentProductsTypesId,
            };
        }
    }
}