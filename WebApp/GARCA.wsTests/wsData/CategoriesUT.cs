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
    public class CategoriesUT : BaseUT<Categories,CategoriesValidations>
    {
        public override Categories CreateObj()
        {
            var categoriesTypesId = new CategoriesTypesRepository().Insert(new CategoriesTypesUT().CreateObj()).Result;

            return new Categories()
            {
                Id = int.MaxValue,
                Description = "TestDescrip",
                CategoriesTypesId = categoriesTypesId,
            };
        }
    }
}