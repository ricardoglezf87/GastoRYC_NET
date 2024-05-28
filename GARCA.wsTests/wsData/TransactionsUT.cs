using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class TransactionsUT : BaseUT<Transactions, TransactionsValidations, TransactionsRepository>
    {
        public override Transactions MakeChange(Transactions obj)
        {
            obj.Memo = getNextWord();
            return obj;
        }

        [Test]
        public void ValidarCalculoBalance_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lCategories = new List<Categories>();
                var lPersons = new List<Persons>();
                var lTransactionsStatus = new List<TransactionsStatus>();

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var person = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
                    lPersons.Add(person);

                    var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
                    lCategories.Add(category);

                    var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(account);

                    var transactionsStatus = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;
                    lTransactionsStatus.Add(transactionsStatus);
                }

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Transactions transaction = new Transactions()
                    {
                        Id = 0,
                        Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                        AccountsId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        PersonsId = lPersons[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lCategories[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                        TransactionsStatusId = lTransactionsStatus[new Random().Next(0, N_INITIAL_MASTER)].Id,
                    };

                    var result = repository.Save(transaction).Result;
                }

                foreach (var account in lAccounts)
                {
                    TestBalanceListAccounts(account);
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        private void TestBalanceListAccounts(Accounts acc)
        {
            var lTransactions = repository.GetByAccount(acc.Id).Result;

            if (lTransactions != null && lTransactions.Any())
            {
                Decimal total = lTransactions.Sum(x => x.AmountIn - x.AmountOut) ?? 0;
                Decimal last = lTransactions.OrderBy(x => x.Orden).Last().Balance ?? 0;

                Assert.That(total, Is.EqualTo(last), $"TestAccount: {acc.Id}");
            }
        }

        [Test]
        public void ValidarCalculoBalanceWithUpdate_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lCategories = new List<Categories>();
                var lPersons = new List<Persons>();
                var lTransactionsStatus = new List<TransactionsStatus>();

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var person = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
                    lPersons.Add(person);

                    var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
                    lCategories.Add(category);

                    var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(account);

                    var transactionsStatus = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;
                    lTransactionsStatus.Add(transactionsStatus);
                }

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Transactions transaction = new Transactions()
                    {
                        Id = 0,
                        Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                        AccountsId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        PersonsId = lPersons[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lCategories[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                        TransactionsStatusId = lTransactionsStatus[new Random().Next(0, N_INITIAL_MASTER)].Id,
                    };


                    var val = validator.Validate(transaction);
                    if (!val.IsValid)
                        throw new Exception(val.Errors[0].ErrorMessage);

                    var result = repository.Save(transaction).Result;
                }

                for (int i = 0; i < N_CHANGED_ITEM; i++)
                {
                    var accountsId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Id;
                    var lTransactions = repository.GetByAccount(accountsId).Result?.ToList();

                    if (lTransactions != null && lTransactions.Any())
                    {
                        var transaction = lTransactions[new Random().Next(0, lTransactions.Count() - 1)] ?? new Transactions();

                        var val = validator.Validate(transaction);
                        if (!val.IsValid)
                            throw new Exception(val.Errors[0].ErrorMessage);

                        transaction.AmountIn = getNextDecimal();
                        transaction.AmountOut = getNextDecimal();

                        _ = repository.Save(transaction).Result;
                    }
                }

                foreach (var account in lAccounts)
                {
                    TestBalanceListAccounts(account);
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidarCalculoBalanceWithDelete_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lCategories = new List<Categories>();
                var lPersons = new List<Persons>();
                var lTransactionsStatus = new List<TransactionsStatus>();
                var lTransactions = new List<Transactions>();

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var person = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
                    lPersons.Add(person);

                    var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
                    lCategories.Add(category);

                    var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(account);

                    var transactionsStatus = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;
                    lTransactionsStatus.Add(transactionsStatus);
                }

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Transactions transaction = new Transactions()
                    {
                        Id = 0,
                        Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                        AccountsId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        PersonsId = lPersons[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lCategories[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                        TransactionsStatusId = lTransactionsStatus[new Random().Next(0, N_INITIAL_MASTER)].Id,
                    };

                    lTransactions.Add(repository.Save(transaction).Result);
                }

                for (int i = 0; i < N_CHANGED_ITEM; i++)
                {
                    int id = new Random().Next(0, lTransactions.Count - 1);
                    var transactions = lTransactions[id];
                    var result = repository.Delete(transactions).Result;
                    lTransactions.RemoveAt(id);
                }

                foreach (var account in lAccounts)
                {
                    TestBalanceListAccounts(account);
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidarCalculoBalanceTranf_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lPersons = new List<Persons>();
                var lTransactionsStatus = new List<TransactionsStatus>();

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var person = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
                    lPersons.Add(person);

                    var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(account);

                    var transactionsStatus = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;
                    lTransactionsStatus.Add(transactionsStatus);
                }

                int rTotal = repository.GetAll()?.Result?.Count() ?? 0;

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Transactions transaction = new Transactions()
                    {
                        Id = 0,
                        Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                        AccountsId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        PersonsId = lPersons[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Categoryid,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                        TransactionsStatusId = lTransactionsStatus[new Random().Next(0, N_INITIAL_MASTER)].Id,
                    };

                    _ = repository.Save(transaction).Result;
                }

                int rActual = repository.GetAll()?.Result?.Count() ?? 0;

                Assert.That(rTotal + (N_INSERT_ITEM * 2), Is.EqualTo(rActual));

                foreach (var account in lAccounts)
                {
                    TestBalanceListAccounts(account);
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidarCalculoBalanceTranfWithUpdate_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lPersons = new List<Persons>();
                var lTransactionsStatus = new List<TransactionsStatus>();

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var person = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
                    lPersons.Add(person);

                    var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(account);

                    var transactionsStatus = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;
                    lTransactionsStatus.Add(transactionsStatus);
                }

                int rTotal = repository.GetAll()?.Result?.Count() ?? 0;

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Transactions transaction = new Transactions()
                    {
                        Id = 0,
                        Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                        AccountsId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        PersonsId = lPersons[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Categoryid,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                        TransactionsStatusId = lTransactionsStatus[new Random().Next(0, N_INITIAL_MASTER)].Id,
                    };

                    _ = repository.Save(transaction).Result;
                }

                int rActual = repository.GetAll()?.Result?.Count() ?? 0;

                Assert.That(rTotal + (N_INSERT_ITEM * 2), Is.EqualTo(rActual));

                for (int i = 0; i < N_CHANGED_ITEM; i++)
                {
                    var accountsId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Id;
                    var lTransactions = repository.GetByAccount(accountsId).Result?.ToList();

                    if (lTransactions != null && lTransactions.Any())
                    {
                        var transaction = lTransactions[new Random().Next(0, lTransactions.Count - 1)] ?? new Transactions();

                        var val = validator.Validate(transaction);
                        if (!val.IsValid)
                            throw new Exception(val.Errors[0].ErrorMessage);

                        transaction.AmountIn = getNextDecimal();
                        transaction.AmountOut = getNextDecimal();

                        _ = repository.Save(transaction).Result;
                    }
                }

                rActual = repository.GetAll()?.Result?.Count() ?? 0;
                Assert.That(rTotal + (N_INSERT_ITEM * 2), Is.EqualTo(rActual));

                foreach (var account in lAccounts)
                {
                    TestBalanceListAccounts(account);
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidarCalculoBalanceTranfWithRemoveCategoryTransfer_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lPersons = new List<Persons>();
                var lTransactionsStatus = new List<TransactionsStatus>();
                var lTransactions = new List<Transactions>();

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var person = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
                    lPersons.Add(person);

                    var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(account);

                    var transactionsStatus = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;
                    lTransactionsStatus.Add(transactionsStatus);
                }

                int rTotal = repository.GetAll()?.Result?.Count() ?? 0;

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Transactions transaction = new Transactions()
                    {
                        Id = 0,
                        Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                        AccountsId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        PersonsId = lPersons[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Categoryid,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                        TransactionsStatusId = lTransactionsStatus[new Random().Next(0, N_INITIAL_MASTER)].Id,
                    };

                    lTransactions.Add(repository.Save(transaction).Result);
                }

                int rActual = repository.GetAll()?.Result?.Count() ?? 0;
                Assert.That(rTotal + (N_INSERT_ITEM * 2), Is.EqualTo(rActual));

                for (int i = 0; i < N_CHANGED_ITEM; i++)
                {
                    int id = new Random().Next(0, lTransactions.Count - 1);
                    var transactions = lTransactions[id];
                    transactions.CategoriesId = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result.Id;

                    _ = repository.Save(transactions).Result;

                    transactions = repository.GetById(transactions.TranferId ?? -99).Result;
                    Assert.That(transactions, Is.Null);

                    lTransactions.RemoveAt(id);
                }

                rActual = repository.GetAll()?.Result?.Count() ?? 0;
                Assert.That(rTotal + (N_INSERT_ITEM + N_CHANGED_ITEM), Is.EqualTo(rActual));

                foreach (var account in lAccounts)
                {
                    TestBalanceListAccounts(account);
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidarCalculoBalanceTranfWithDelete_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lCategories = new List<Categories>();
                var lPersons = new List<Persons>();
                var lTransactionsStatus = new List<TransactionsStatus>();
                var lTransactions = new List<Transactions>();

                for (int i = 0; i < N_INITIAL_MASTER; i++)
                {
                    var person = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
                    lPersons.Add(person);

                    var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(account);

                    var transactionsStatus = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;
                    lTransactionsStatus.Add(transactionsStatus);
                }

                int rTotal = repository.GetAll()?.Result?.Count() ?? 0;

                for (int i = 0; i < N_INSERT_ITEM; i++)
                {
                    Transactions transaction = new Transactions()
                    {
                        Id = 0,
                        Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                        AccountsId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        PersonsId = lPersons[new Random().Next(0, N_INITIAL_MASTER)].Id,
                        CategoriesId = lAccounts[new Random().Next(0, N_INITIAL_MASTER)].Categoryid,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                        TransactionsStatusId = lTransactionsStatus[new Random().Next(0, N_INITIAL_MASTER)].Id,
                    };

                    lTransactions.Add(repository.Save(transaction).Result);
                }

                int rActual = repository.GetAll()?.Result?.Count() ?? 0;
                Assert.That(rTotal + (N_INSERT_ITEM * 2), Is.EqualTo(rActual));

                for (int i = 0; i < N_CHANGED_ITEM; i++)
                {
                    int id = new Random().Next(0, lTransactions.Count - 1);
                    var transactions = lTransactions[id];
                    _ = repository.Delete(transactions).Result;
                    lTransactions.RemoveAt(id);
                }

                rActual = repository.GetAll()?.Result?.Count() ?? 0;
                Assert.That(rTotal + N_INSERT_ITEM, Is.EqualTo(rActual));

                foreach (var account in lAccounts)
                {
                    TestBalanceListAccounts(account);
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        public override Transactions CreateObj()
        {
            var person = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
            var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
            var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
            var statustran = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;

            return new Transactions()
            {
                Id = 0,
                Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                AccountsId = account.Id,
                PersonsId = person.Id,
                CategoriesId = category.Id,
                AmountIn = getNextDecimal(),
                AmountOut = getNextDecimal(),
                TransactionsStatusId = statustran.Id,
            };
        }
    }
}