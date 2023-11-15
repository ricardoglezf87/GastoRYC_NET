using Dapper;
using GARCA.Data.Services;
using GARCA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class CategoriesTypesManager : ManagerBase<CategoriesTypes, Int32>
    {
    }
}
