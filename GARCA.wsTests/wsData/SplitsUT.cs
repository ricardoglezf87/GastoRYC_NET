using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;

using Microsoft.AspNetCore.Http.HttpResults;

using System.Net;
using static GARCA.Utils.Enums.EnumCategories;

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


        [Test]
        public void ValidarCalculoTotalTrans_Ok()
        {
            try
            {
                var lCategories = new List<Categories>();
                var lTransactions = new List<Transactions>();

                for (int i = 0; i < 5; i++)
                {
                    var trans = new TransactionsRepository().Save(new TransactionsUT().CreateObj()).Result;
                    lTransactions.Add(trans);

                    var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
                    lCategories.Add(category);                  
                }

                for (int i = 0; i < 100; i++)
                {
                   Splits splits = new Splits()
                   {
                       Id = 0,
                       TransactionsId = lTransactions[new Random().Next(0, 5)].Id,
                       CategoriesId = lCategories[new Random().Next(0, 5)].Id,
                       AmountIn = getNextDecimal(),
                       AmountOut = getNextDecimal(),
                   };

                    var val = validator.Validate(splits);
                    if (!val.IsValid)
                        throw new Exception(val.Errors[0].ErrorMessage);

                    var result = SplitsAPI.Create(splits, repository, validator).Result;
                    Assert.That((HttpStatusCode)getOkResult(result).StatusCode, Is.EqualTo(HttpStatusCode.OK));
                }

                foreach (var t in lTransactions)
                {
                    var transaction = new TransactionsRepository().GetById(t.Id).Result;

                    Assert.That(transaction.CategoriesId, Is.EqualTo((int)ESpecialCategories.Split), $"TestTransCategory: {transaction.Id}");

                    var result = SplitsAPI.Get($"TransactionsId={transaction.Id}", repository).Result;                    

                    if (result is Ok<ResponseAPI> okResult)
                    {
                        var lSplits = (List<Splits?>?)okResult.Value.Result ?? new List<Splits?>();
                        Decimal total = lSplits.Sum(x => x.Amount) ?? 0;                        
                        Assert.That(total, Is.EqualTo(transaction.Amount), $"TestTrans: {transaction.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidarCalculoTotalTransAfterUpdate_Ok()
        {
            try
            {
                var lCategories = new List<Categories>();
                var lTransactions = new List<Transactions>();
                var lSplits = new List<Splits>();

                for (int i = 0; i < 5; i++)
                {
                    var trans = new TransactionsRepository().Save(new TransactionsUT().CreateObj()).Result;
                    lTransactions.Add(trans);

                    var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
                    lCategories.Add(category);
                }

                for (int i = 0; i < 100; i++)
                {
                    Splits splits = new Splits()
                    {
                        Id = 0,
                        TransactionsId = lTransactions[new Random().Next(0, 5)].Id,
                        CategoriesId = lCategories[new Random().Next(0, 5)].Id,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                    };

                    var val = validator.Validate(splits);
                    if (!val.IsValid)
                        throw new Exception(val.Errors[0].ErrorMessage);

                    var result = SplitsAPI.Create(splits, repository, validator).Result;
                    var okResult = getOkResult(result);
                    lSplits.Add((Splits)okResult.Value.Result);
                    Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                }

                for (int i = 0; i < 50; i++)
                {
                    int id = new Random().Next(0, lTransactions.Count - 1);
                    var splits = lSplits[id];
                    
                    splits.AmountIn = getNextDecimal();
                    splits.AmountOut = getNextDecimal();

                    var result = SplitsAPI.Update(splits, repository, validator).Result;
                    getOkResult(result);
                    
                    lSplits.RemoveAt(id);
                }

                foreach (var t in lTransactions)
                {
                    var transaction = new TransactionsRepository().GetById(t.Id).Result;

                    Assert.That(transaction.CategoriesId, Is.EqualTo((int)ESpecialCategories.Split), $"TestTransCategory: {transaction.Id}");

                    var result = SplitsAPI.Get($"TransactionsId={transaction.Id}", repository).Result;

                    if (result is Ok<ResponseAPI> okResult)
                    {
                        lSplits = (List<Splits?>?)okResult.Value.Result ?? new List<Splits?>();
                        Decimal total = lSplits.Sum(x => x.Amount) ?? 0;
                        Assert.That(total, Is.EqualTo(transaction.Amount), $"TestTrans: {transaction.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidarCalculoTotalTransAfterDelete_Ok()
        {
            try
            {
                var lCategories = new List<Categories>();
                var lTransactions = new List<Transactions>();
                var lSplits = new List<Splits>();

                for (int i = 0; i < 5; i++)
                {
                    var trans = new TransactionsRepository().Save(new TransactionsUT().CreateObj()).Result;
                    lTransactions.Add(trans);

                    var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
                    lCategories.Add(category);
                }

                for (int i = 0; i < 100; i++)
                {
                    Splits splits = new Splits()
                    {
                        Id = 0,
                        TransactionsId = lTransactions[new Random().Next(0, 5)].Id,
                        CategoriesId = lCategories[new Random().Next(0, 5)].Id,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                    };

                    var val = validator.Validate(splits);
                    if (!val.IsValid)
                        throw new Exception(val.Errors[0].ErrorMessage);

                    var result = SplitsAPI.Create(splits, repository, validator).Result;
                    var okResult = getOkResult(result);
                    lSplits.Add((Splits)okResult.Value.Result);
                    Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                }

                for (int i = 0; i < 50; i++)
                {
                    int id = new Random().Next(0, lTransactions.Count - 1);
                    var splits = lSplits[id];

                    var result = SplitsAPI.Delete(splits.Id.ToString(), repository).Result;
                    getOkResult(result);

                    lSplits.RemoveAt(id);
                }

                foreach (var t in lTransactions)
                {
                    var transaction = new TransactionsRepository().GetById(t.Id).Result;

                    Assert.That(transaction.CategoriesId, Is.EqualTo((int)ESpecialCategories.Split), $"TestTransCategory: {transaction.Id}");

                    var result = SplitsAPI.Get($"TransactionsId={transaction.Id}", repository).Result;

                    if (result is Ok<ResponseAPI> okResult)
                    {
                        lSplits = (List<Splits?>?)okResult.Value.Result ?? new List<Splits?>();
                        Decimal total = lSplits.Sum(x => x.Amount) ?? 0;
                        Assert.That(total, Is.EqualTo(transaction.Amount), $"TestTrans: {transaction.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
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