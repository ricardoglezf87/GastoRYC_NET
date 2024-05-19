using Dapper;
using GARCA.wsData.Repositories;
using GARCA.Models;
using GARCA.Utils.Extensions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class PersonsService : ServiceBase<PersonsRepository, Persons>
    {        
    }
}


