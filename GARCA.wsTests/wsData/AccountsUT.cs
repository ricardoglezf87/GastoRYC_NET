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
                    throw new Exception(val.Errors[0].ErrorMessage);

                var result = AcountsAPI.Create(obj, repository, validator).Result;
                var accounts = (Accounts?)getOkResult(result).Value.Result;
                Assert.That(accounts != null && accounts.Categoryid != null);

                var categoryId = accounts.Categoryid.ToString() ?? throw new Exception("No se ha creado la categoría correctamente");
                result = CategoriesAPI.GetById(categoryId, new CategoriesRepository()).Result;
                var categories = (Categories?)getOkResult(result).Value.Result;

                Assert.That(categories?.Description == $"[{accounts.Description}]");
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
                    throw new Exception(val.Errors[0].ErrorMessage);

                var result = AcountsAPI.Create(obj, repository, validator).Result;
                var accounts = (Accounts?)getOkResult(result).Value.Result;
                Assert.That(accounts != null && accounts.Categoryid != null);

                var categoryId = accounts.Categoryid.ToString() ?? throw new Exception("No se ha creado la categoría correctamente");
                result = CategoriesAPI.GetById(categoryId, new CategoriesRepository()).Result;
                var categories = (Categories?)getOkResult(result).Value.Result;

                Assert.That(categories?.Description == $"[{accounts.Description}]");

                // Actualizar la descripción
                accounts.Description = "Cambio";
                result = AcountsAPI.Update(accounts, repository, validator).Result;
                Assert.That((HttpStatusCode)getOkResult(result).StatusCode, Is.EqualTo(HttpStatusCode.OK));

                // Verificar que la descripción se haya actualizado correctamente
                result = CategoriesAPI.GetById(categoryId, new CategoriesRepository()).Result;
                categories = (Categories?)getOkResult(result).Value.Result;
                Assert.That(categories?.Description == $"[{accounts.Description}]");
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
                    throw new Exception(val.Errors[0].ErrorMessage);

                var result = AcountsAPI.Create(obj, repository, validator).Result;
                var accounts = (Accounts?)getOkResult(result).Value.Result;
                Assert.That(accounts != null && accounts.Categoryid != null);

                string categoryId = accounts.Categoryid.ToString() ?? throw new Exception("No se ha creado la categoría correctamente");

                // Eliminar la cuenta
                result = AcountsAPI.Delete(accounts.Id.ToString(), repository).Result;
                Assert.That((HttpStatusCode)getOkResult(result).StatusCode, Is.EqualTo(HttpStatusCode.OK));

                // Verificar que la categoría ya no existe
                result = CategoriesAPI.GetById(categoryId, new CategoriesRepository()).Result;
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