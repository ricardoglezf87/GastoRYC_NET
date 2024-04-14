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
    public class TransactionsStatusUT : BaseUT<TransactionsStatus,TransactionsStatusValidations>
    {
        public override TransactionsStatus MakeChange(TransactionsStatus obj)
        {
            obj.Description = "TestDescripUpdate";
            return obj;
        }

        public override TransactionsStatus CreateObj()
        {
            return new TransactionsStatus()
            {
                Id = int.MaxValue,
                Description = "TestDescrip"
            };
        }
    }
}