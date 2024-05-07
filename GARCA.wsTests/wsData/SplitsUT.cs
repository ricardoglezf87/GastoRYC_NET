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
    public class SplitsUT : BaseUT<Splits, SplitsValidations,SplitsRepository>
    {
        public override Splits MakeChange(Splits obj)
        {
            obj.Memo = "TestDescripUpdate";
            return obj;
        }

        public override Splits CreateObj()
        {
            var categoryid = new CategoriesRepository().Insert(new CategoriesUT().CreateObj()).Result;
            var transid = new TransactionsRepository().Insert(new TransactionsUT().CreateObj()).Result;

            return new Splits()
            {
                Id = 0,
                CategoriesId = categoryid,
                TransactionsId = transid,
                AmountIn = getNextDecimal(),
                AmountOut = getNextDecimal()
            };
        }
    }
}