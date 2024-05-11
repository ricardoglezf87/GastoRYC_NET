using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    public class AccountsUT : BaseUT<Accounts,AccountsValidations, AccountsRepository>
    {
        public override Accounts MakeChange(Accounts obj)
        {
            obj.Description = "TestDescripUpdate";
            return obj;
        }

        [Test]
        public void CreateCategory_Ok()
        {
            try
            {
                var obj = CreateObj();

                var val = validator.Validate(obj);

                if (!val.IsValid)
                {
                    throw new Exception(val.Errors[0].ErrorMessage);
                }

                var result = AcountsAPI.Create(obj, repository, validator).Result;

                var okResult = getOkResult(result);

                Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Accounts? accounts = (Accounts?)okResult.Value.Result;

                Assert.That(accounts != null);

                Assert.That(accounts.Categoryid != null);

                result = CategoriesAPI.GetById(accounts.Categoryid.ToString() ?? "-99", new CategoriesRepository()).Result;

                okResult = getOkResult(result);

                Categories? categories = (Categories?)okResult.Value.Result;

                Assert.That(categories.Description == "[" + accounts.Description + "]");
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void UpdateCategory_Ok()
        {
            try
            {
                var obj = CreateObj();

                var val = validator.Validate(obj);

                if (!val.IsValid)
                {
                    throw new Exception(val.Errors[0].ErrorMessage);
                }

                var result = AcountsAPI.Create(obj, repository, validator).Result;

                var okResult = getOkResult(result);

                Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Accounts? accounts = (Accounts?)okResult.Value.Result;

                Assert.That(accounts != null);

                Assert.That(accounts.Categoryid != null);

                result = CategoriesAPI.GetById(accounts.Categoryid.ToString() ?? "-99", new CategoriesRepository()).Result;

                okResult = getOkResult(result);

                Categories? categories = (Categories?)okResult.Value.Result;

                Assert.That(categories.Description == "[" + accounts.Description + "]");

                accounts.Description = "Cambio";

                result = AcountsAPI.Update(accounts, repository, validator).Result;

                okResult = getOkResult(result);

                result = CategoriesAPI.GetById(accounts.Categoryid.ToString() ?? "-99", new CategoriesRepository()).Result;

                okResult = getOkResult(result);

                categories = (Categories?)okResult.Value.Result;

                Assert.That(categories.Description == "[" + accounts.Description + "]");
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void DeleteCategory_Ok()
        {
            try
            {
                var obj = CreateObj();

                var val = validator.Validate(obj);

                if (!val.IsValid)
                {
                    throw new Exception(val.Errors[0].ErrorMessage);
                }

                var result = AcountsAPI.Create(obj, repository, validator).Result;

                var okResult = getOkResult(result);

                Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Accounts? accounts = (Accounts?)okResult.Value.Result;

                Assert.That(accounts != null);

                Assert.That(accounts.Categoryid != null);

                string categoryid = accounts.Categoryid.ToString() ?? "-99";

                result = AcountsAPI.Delete(accounts.Id.ToString(), repository).Result;

                okResult = getOkResult(result);

                Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                result = CategoriesAPI.GetById(categoryid, new CategoriesRepository()).Result;

                Assert.That((HttpStatusCode)getNotFoundResult(result).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }


        public override Accounts CreateObj()
        {
            var accountsTypesId = new AccountsTypesRepository().Insert(new AccountsTypesUT().CreateObj()).Result;

            return new Accounts()
            {
                Id = 0,
                Description = "TestDescrip",
                AccountsTypesId = accountsTypesId,
            };
        }
    }
}