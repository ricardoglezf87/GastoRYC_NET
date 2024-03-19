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
    public class SplitsUT : BaseUT<Splits, SplitsValidations>
    {
        public override Splits CreateObj()
        {
            return new Splits()
            {
                Id = int.MaxValue,
                CategoriesId = int.MaxValue,
                TransactionsId = int.MaxValue,
                AmountIn = decimal.MaxValue,
                AmountOut = decimal.MaxValue
            };
        }
    }
}