using Dapper;
using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class PersonsService : ServiceBase<PersonsManager, Persons>
    {        
    }
}


