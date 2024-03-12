using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

using System.Net;

namespace GARCA.wsDataTest
{
    [TestFixture]
    public class PersonsUT: BaseUT<Persons>
    {       
    }
}