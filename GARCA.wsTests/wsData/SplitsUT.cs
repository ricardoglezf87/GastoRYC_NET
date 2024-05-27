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
    public class SplitsUT : BaseUT<Splits, SplitsValidations, SplitsRepository>
    {
        public override Splits MakeChange(Splits obj)
        {
            obj.Memo = getNextWord();
            return obj;
        }

        [Test]
        public void ValidarTranferSplit_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lTransactions = new List<Transactions>();
                var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var trans = new TransactionsUT().CreateObj();
                    trans.AccountsId = account.Id;
                    trans = new TransactionsRepository().Save(trans).Result;
                    lTransactions.Add(trans);

                    var accounts = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(accounts);
                }

                int rTotal = new TransactionsRepository().GetAll()?.Result?.Count() ?? 0;

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Splits splits = new Splits()
                    {
                        Id = 0,
                        TransactionsId = lTransactions[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Categoryid,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                    };
                    _ = repository.Save(splits).Result;
                }

                int rActual = new TransactionsRepository().GetAll()?.Result?.Count() ?? 0;
                Assert.That(rTotal + N_INSERT_ITEM, Is.EqualTo(rActual));

                foreach (var t in lTransactions)
                {
                    var transaction = new TransactionsRepository().GetById(t.Id).Result;

                    Assert.That(transaction.CategoriesId, Is.EqualTo((int)ESpecialCategories.Split), $"TestTransCategory: {transaction.Id}");

                    var lSplits = repository.GetbyTransactionid(transaction.Id).Result?.ToList() ?? new List<Splits>();
                    Decimal total = lSplits.Sum(x => x.Amount) ?? 0;
                    Assert.That(total, Is.EqualTo(transaction.Amount), $"TestTrans: {transaction.Id}");
                }

                TestBalanceAccount(account);

                foreach (var acc in lAccounts)
                {
                    TestBalanceAccount(acc);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidarTranferSplitWithUpdate_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lTransactions = new List<Transactions>();
                var lSplits = new List<Splits>();
                var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var trans = new TransactionsUT().CreateObj();
                    trans.AccountsId = account.Id;
                    trans = new TransactionsRepository().Save(trans).Result;
                    lTransactions.Add(trans);

                    var accounts = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(accounts);
                }

                int rTotal = new TransactionsRepository().GetAll()?.Result?.Count() ?? 0;

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Splits splits = new Splits()
                    {
                        Id = 0,
                        TransactionsId = lTransactions[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Categoryid,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                    };
                    lSplits.Add(repository.Save(splits).Result);
                }

                int rActual = new TransactionsRepository().GetAll()?.Result?.Count() ?? 0;
                Assert.That(rTotal + N_INSERT_ITEM, Is.EqualTo(rActual));

                for (int i = 0; i < N_CHANGED_ITEM; i++)
                {
                    int id = new Random().Next(0, lTransactions.Count - 1);
                    var splits = lSplits[id];
                    splits.AmountIn = getNextDecimal();
                    splits.AmountOut = getNextDecimal();
                    _ = repository.Save(splits).Result;
                    lSplits.RemoveAt(id);
                }

                rActual = new TransactionsRepository().GetAll()?.Result?.Count() ?? 0;
                Assert.That(rTotal + N_INSERT_ITEM, Is.EqualTo(rActual));

                foreach (var t in lTransactions)
                {
                    var transaction = new TransactionsRepository().GetById(t.Id).Result;

                    Assert.That(transaction.CategoriesId, Is.EqualTo((int)ESpecialCategories.Split), $"TestTransCategory: {transaction.Id}");

                    lSplits = repository.GetbyTransactionid(transaction.Id).Result?.ToList() ?? new List<Splits>();
                    Decimal total = lSplits.Sum(x => x.Amount) ?? 0;
                    Assert.That(total, Is.EqualTo(transaction.Amount), $"TestTrans: {transaction.Id}");
                }

                TestBalanceAccount(account);

                foreach (var acc in lAccounts)
                {
                    TestBalanceAccount(acc);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidarTranferSplitWithDelete_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lTransactions = new List<Transactions>();
                var lSplits = new List<Splits>();
                var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var trans = new TransactionsUT().CreateObj();
                    trans.AccountsId = account.Id;
                    trans = new TransactionsRepository().Save(trans).Result;
                    lTransactions.Add(trans);

                    var accounts = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(accounts);
                }

                int rTotal = new TransactionsRepository().GetAll()?.Result?.Count() ?? 0;

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Splits splits = new Splits()
                    {
                        Id = 0,
                        TransactionsId = lTransactions[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Categoryid,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                    };
                    lSplits.Add(repository.Save(splits).Result);
                }

                int rActual = new TransactionsRepository().GetAll()?.Result?.Count() ?? 0;
                Assert.That(rTotal + N_INSERT_ITEM, Is.EqualTo(rActual));

                for (int i = 0; i < N_CHANGED_ITEM; i++)
                {
                    int id = new Random().Next(0, lTransactions.Count - 1);
                    var splits = lSplits[id];
                    _ = repository.Delete(splits).Result;

                    Assert.That(new TransactionsRepository().GetById(splits.TranferId ?? -99).Result, Is.Null);

                    lSplits.RemoveAt(id);
                }

                rActual = new TransactionsRepository().GetAll()?.Result?.Count() ?? 0;
                Assert.That(rTotal + N_CHANGED_ITEM, Is.EqualTo(rActual));

                foreach (var t in lTransactions)
                {
                    var transaction = new TransactionsRepository().GetById(t.Id).Result;

                    Assert.That(transaction.CategoriesId, Is.EqualTo((int)ESpecialCategories.Split), $"TestTransCategory: {transaction.Id}");

                    lSplits = repository.GetbyTransactionid(transaction.Id).Result?.ToList() ?? new List<Splits>();
                    Decimal total = lSplits.Sum(x => x.Amount) ?? 0;
                    Assert.That(total, Is.EqualTo(transaction.Amount), $"TestTrans: {transaction.Id}");
                }

                TestBalanceAccount(account);

                foreach (var acc in lAccounts)
                {
                    TestBalanceAccount(acc);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidarCalculoTotalTrans_Ok()
        {
            try
            {
                var lCategories = new List<Categories>();
                var lTransactions = new List<Transactions>();
                var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var trans = new TransactionsUT().CreateObj();
                    trans.AccountsId = account.Id;
                    trans = new TransactionsRepository().Save(trans).Result;
                    lTransactions.Add(trans);

                    var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
                    lCategories.Add(category);
                }

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Splits splits = new Splits()
                    {
                        Id = 0,
                        TransactionsId = lTransactions[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lCategories[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                    };
                    _ = repository.Save(splits).Result;
                }

                foreach (var t in lTransactions)
                {
                    var transaction = new TransactionsRepository().GetById(t.Id).Result;

                    Assert.That(transaction.CategoriesId, Is.EqualTo((int)ESpecialCategories.Split), $"TestTransCategory: {transaction.Id}");

                    var lSplits = repository.GetbyTransactionid(transaction.Id).Result?.ToList() ?? new List<Splits>();
                    Decimal total = lSplits.Sum(x => x.Amount) ?? 0;
                    Assert.That(total, Is.EqualTo(transaction.Amount), $"TestTrans: {transaction.Id}");
                }

                TestBalanceAccount(account);
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
                var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var trans = new TransactionsUT().CreateObj();
                    trans.AccountsId = account.Id;
                    trans = new TransactionsRepository().Save(trans).Result;
                    lTransactions.Add(trans);

                    var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
                    lCategories.Add(category);
                }

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Splits splits = new Splits()
                    {
                        Id = 0,
                        TransactionsId = lTransactions[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lCategories[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                    };

                    lSplits.Add(repository.Save(splits).Result);
                }

                for (int i = 0; i < N_CHANGED_ITEM; i++)
                {
                    int id = new Random().Next(0, lTransactions.Count - 1);
                    var splits = lSplits[id];

                    splits.AmountIn = getNextDecimal();
                    splits.AmountOut = getNextDecimal();

                    _ = repository.Save(splits).Result;

                    lSplits.RemoveAt(id);
                }

                foreach (var t in lTransactions)
                {
                    var transaction = new TransactionsRepository().GetById(t.Id).Result;

                    Assert.That(transaction.CategoriesId, Is.EqualTo((int)ESpecialCategories.Split), $"TestTransCategory: {transaction.Id}");

                    lSplits = repository.GetbyTransactionid(transaction.Id).Result?.ToList() ?? new List<Splits>();
                    Decimal total = lSplits.Sum(x => x.Amount) ?? 0;
                    Assert.That(total, Is.EqualTo(transaction.Amount), $"TestTrans: {transaction.Id}");
                }

                TestBalanceAccount(account);
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

                var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var trans = new TransactionsUT().CreateObj();
                    trans.AccountsId = account.Id;
                    trans = new TransactionsRepository().Save(trans).Result;
                    lTransactions.Add(trans);

                    var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
                    lCategories.Add(category);
                }

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Splits splits = new Splits()
                    {
                        Id = 0,
                        TransactionsId = lTransactions[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lCategories[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                    };

                    lSplits.Add(repository.Save(splits).Result);
                }

                for (int i = 0; i < N_CHANGED_ITEM; i++)
                {
                    int id = new Random().Next(0, lTransactions.Count - 1);
                    var splits = lSplits[id];

                    _ = repository.Delete(splits).Result;

                    lSplits.RemoveAt(id);
                }

                foreach (var t in lTransactions)
                {
                    var transaction = new TransactionsRepository().GetById(t.Id).Result;

                    Assert.That(transaction.CategoriesId, Is.EqualTo((int)ESpecialCategories.Split), $"TestTransCategory: {transaction.Id}");

                    lSplits = repository.GetbyTransactionid(transaction.Id).Result?.ToList() ?? new List<Splits>();
                    Decimal total = lSplits.Sum(x => x.Amount) ?? 0;
                    Assert.That(total, Is.EqualTo(transaction.Amount), $"TestTrans: {transaction.Id}");
                }
                
                TestBalanceAccount(account);

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        private void TestBalanceAccount(Accounts accounts)
        {
            var lTransactions = new TransactionsRepository().GetByAccount(accounts.Id).Result?.ToList() ?? new List<Transactions>();

            Decimal totalT = lTransactions.Sum(x => x.AmountIn - x.AmountOut) ?? 0;
            Decimal lastT = lTransactions.OrderBy(x => x.Orden).Last().Balance ?? 0;

            Assert.That(totalT, Is.EqualTo(lastT), $"TestAccount: {accounts.Id}");
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