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
    public class CategoriesManager : ManagerBase<Categories, Int32>
    {
        protected override string GetGeneralQuery()
        {
            return @"
                    select * 
                    from Categories
                        inner join CategoriesTypes on CategoriesTypes.Id = Categories.categoriesTypesid
                    ";
        }

        public async override Task<IEnumerable<Categories>?> GetAll()
        {
            return await iRycContextService.getConnection().QueryAsync<Categories, CategoriesTypes, Categories>(
                GetGeneralQuery()
                , (a, at) =>
                {
                    a.CategoriesTypes = at;
                    return a;
                });

        }
    }
}
