using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;

using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class TransactionsUT : BaseUT<Transactions,TransactionsValidations,TransactionsRepository>
    {
        public override Transactions MakeChange(Transactions obj)
        {
            obj.Memo = "TestDescripUpdate";
            return obj;
        }

        [Test]
        public void ValideBalance_Ok()
        {
            try
            {
                var lAccounts = new List<Accounts>();
                var lCategories = new List<Categories>();
                var lPersons = new List<Persons>();
                var lTransactionsStatus = new List<TransactionsStatus>();
                var lTransactions = new List<Transactions>();

                for(int i=0;i<5;i++)
                {
                    var persson = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
                    lPersons.Add(persson);

                    var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
                    lCategories.Add(category);
                    
                    var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
                    lAccounts.Add(account);
                    
                    var transactionsStatus = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;
                    lTransactionsStatus.Add(transactionsStatus);
                }

                for (int i = 0; i < 5; i++)
                {
                    Transactions transaction = new Transactions(){
                        Id = 0,
                        Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                        AccountsId = lAccounts[new Random().Next(0, 5)].Id,
                        PersonsId = lPersons[new Random().Next(0, 5)].Id,
                        CategoriesId = lCategories[new Random().Next(0, 5)].Id,
                        AmountIn = getNextDecimal(),
                        AmountOut = getNextDecimal(),
                        TransactionsStatusId = lTransactionsStatus[new Random().Next(0, 5)].Id,
                    };


                    var val = validator.Validate(transaction);
                    if (!val.IsValid)
                        throw new Exception(val.Errors[0].ErrorMessage);

                    var result = TransactionsAPI.Create(transaction, repository, validator).Result;
                    transaction = (Transactions?)getOkResult(result).Value.Result;
                    lTransactions.Add(transaction);
                }

                Assert.That(true);

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        public override Transactions CreateObj()
        {
            var persson = new PersonsRepository().Save(new PersonsUT().CreateObj()).Result;
            var category = new CategoriesRepository().Save(new CategoriesUT().CreateObj()).Result;
            var account = new AccountsRepository().Save(new AccountsUT().CreateObj()).Result;
            var statustran = new TransactionsStatusRepository().Save(new TransactionsStatusUT().CreateObj()).Result;

            return new Transactions()
            {
                Id = 0,
                Date = DateTime.Now.AddDays(new Random().Next(-30, 30)),
                AccountsId = account.Id,
                PersonsId = persson.Id,
                CategoriesId = category.Id,
                AmountIn = getNextDecimal(),
                AmountOut = getNextDecimal(),
                TransactionsStatusId = statustran.Id,
            };
        }
    }
}