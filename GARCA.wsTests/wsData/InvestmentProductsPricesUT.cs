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
    public class InvestmentProductsPricesUT : BaseUT<InvestmentProductsPrices, InvestmentProductsPricesValidations>
    {
        public override InvestmentProductsPrices MakeChange(InvestmentProductsPrices obj)
        {
            obj.Prices = 10;
            return obj;
        }

        public override InvestmentProductsPrices CreateObj()
        {
            var investmentProductsId = new InvestmentProductsRepository().
                   Insert(new InvestmentProductsUT().CreateObj()).Result;

            return new InvestmentProductsPrices()
            {
                Id = int.MaxValue,
                Date = DateTime.Now,
                InvestmentProductsid = investmentProductsId,
                Prices = decimal.MaxValue,
            };
        }
    }
}