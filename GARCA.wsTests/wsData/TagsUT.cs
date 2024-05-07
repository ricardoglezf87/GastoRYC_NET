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
    public class TagsUT : BaseUT<Tags,TagsValidations,TagsRepository>
    {
        public override Tags MakeChange(Tags obj)
        {
            obj.Description = "TestDescripUpdate";
            return obj;
        }

        public override Tags CreateObj()
        {
            return new Tags()
            {
                Id = 0,
                Description = "TestDescrip"
            };
        }
    }
}