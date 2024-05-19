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
    public class CategoriesTypesUT : BaseUT<CategoriesTypes,CategoriesTypesValidations,CategoriesTypesRepository>
    {
        public override CategoriesTypes MakeChange(CategoriesTypes obj)
        {
            obj.Description = getNextWord();
            return obj;
        }

        public override CategoriesTypes CreateObj()
        {
            return new CategoriesTypes()
            {
                Id = 0,
                Description = getNextWord()
            };
        }
    }
}