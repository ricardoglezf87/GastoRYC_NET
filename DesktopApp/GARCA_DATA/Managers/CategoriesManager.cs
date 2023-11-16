using Dapper;
using Dommel;
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
    public class CategoriesManager : ManagerBase<Categories, Int32>
    {
        public async override Task<IEnumerable<Categories>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<Categories, CategoriesTypes, Categories>();
        }

        public async Task<int> GetNextId()
        {
            return await iRycContextService.getConnection().ExecuteScalarAsync<int>("SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'categories';");
        }
    }
}
