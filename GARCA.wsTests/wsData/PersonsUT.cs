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
    public class PersonsUT : BaseUT<Persons,PersonsValidations>
    {
        public override Persons MakeChange(Persons obj)
        {
            obj.Name = "TestDescripUpdate";
            return obj;
        }

        public override Persons CreateObj()
        {
            var categoryid = new CategoriesRepository().Insert(new CategoriesUT().CreateObj()).Result;

            return new Persons()
            {
                Id = 0,
                Name = "TestDescrip",
                CategoriesId = categoryid,
            };
        }
    }
}