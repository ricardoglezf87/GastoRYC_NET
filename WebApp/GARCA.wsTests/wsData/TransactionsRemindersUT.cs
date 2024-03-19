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
    public class TransactionsRemindersUT : BaseUT<TransactionsReminders,TransactionsRemindersValidations>
    {
        public override TransactionsReminders CreateObj()
        {
            return new TransactionsReminders()
            {
                Id = int.MaxValue,
                Date = DateTime.Now,
                AccountsId = int.MaxValue,
                CategoriesId = int.MaxValue,
                AmountIn = decimal.MaxValue,
                AmountOut = decimal.MaxValue,
                TransactionsStatusId = int.MaxValue,
            };
        }
    }
}