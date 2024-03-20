using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using GARCA_DATA.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class TransactionsUT : BaseUT<Transactions,TransactionsValidations>
    {
        public override Transactions MakeChange(Transactions obj)
        {
            obj.Memo = "TestDescripUpdate";
            return obj;
        }

        public override Transactions CreateObj()
        {
            var perssonid = new PersonsRepository().Insert(new PersonsUT().CreateObj()).Result;
            var categoryid = new CategoriesRepository().Insert(new CategoriesUT().CreateObj()).Result;
            var accountid = new AccountsRepository().Insert(new AccountsUT().CreateObj()).Result;
            var statustid = new TransactionsStatusRepository().Insert(new TransactionsStatusUT().CreateObj()).Result;

            return new Transactions()
            {
                Id = int.MaxValue,
                Date = DateTime.Now,
                AccountsId = accountid,
                PersonsId = perssonid,
                CategoriesId = categoryid,
                AmountIn = decimal.MaxValue,
                AmountOut = decimal.MaxValue,
                TransactionsStatusId = statustid,
            };
        }
    }
}