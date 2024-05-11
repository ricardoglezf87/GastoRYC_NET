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
            obj.Memo = getNextWord();
            return obj;
        }

        public override Splits CreateObj()
        {
            var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
            var trans = new TransactionsRepository().Save(new TransactionsUT().CreateObj()).Result;

            return new Splits()
            {
                Id = 0,
                CategoriesId = category.Id,
                TransactionsId = trans.Id,
                AmountIn = getNextDecimal(),
                AmountOut = getNextDecimal()
            };
        }
    }
}