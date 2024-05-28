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
    public class InvestmentProductsPricesUT : BaseUT<InvestmentProductsPrices, InvestmentProductsPricesValidations,InvestmentProductsPricesRepository>
    {
        public override InvestmentProductsPrices MakeChange(InvestmentProductsPrices obj)
        {
            obj.Prices = getNextDecimal(8);
            return obj;
        }

        public override InvestmentProductsPrices CreateObj()
        {
            var investmentProducts = new InvestmentProductsRepository().
                   Save(new InvestmentProductsUT().CreateObj()).Result;

            return new InvestmentProductsPrices()
            {
                Id = 0,
                Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                InvestmentProductsid = investmentProducts.Id,
                Prices = getNextDecimal(8),
            };
        }
    }
}