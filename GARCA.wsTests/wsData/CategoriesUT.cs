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
    public class CategoriesUT : BaseUT<Categories,CategoriesValidations,CategoriesRepository>
    {
        public override Categories MakeChange(Categories obj)
        {
            obj.Description = "TestDescripUpdate";
            return obj;
        }

        public override Categories CreateObj()
        {
            var categoriesTypes = new CategoriesTypesRepository().Save(new CategoriesTypesUT().CreateObj()).Result;

            return new Categories()
            {
                Id = int.MaxValue,
                Description = "TestDescrip",
                CategoriesTypesId = categoriesTypes.Id,
            };
        }
    }
}