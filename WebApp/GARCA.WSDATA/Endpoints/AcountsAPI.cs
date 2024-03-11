using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.wsData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using static Dapper.SqlMapper;
using static System.Net.WebRequestMethods;

namespace GARCA.wsData.Endpoints
{
    public class AcountsAPI : BaseAPI<Accounts>   
    {
    }
}
