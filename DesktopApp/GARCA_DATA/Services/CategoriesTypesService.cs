using Dapper;
using GARCA.Data.Managers;
using GARCA.Models;
using GARCA_DATA.Services;
using Microsoft.Data.Sqlite;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class CategoriesTypesService : IServiceCache<CategoriesTypes>
    {
        public enum ECategoriesTypes
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        protected override IEnumerable<CategoriesTypes>? GetAllCache()
        {            
            return iRycContextService.getConnection().Query<CategoriesTypes>(
                @"
                    select * 
                    from CategoriesTypes                      
                ").AsEnumerable();

        }

        public HashSet<CategoriesTypes>? GetAllWithoutSpecialTransfer()
        {
            return GetAll()?.Where(x => x.Id is not (int)ECategoriesTypes.Specials and
                    not (int)ECategoriesTypes.Transfers).ToHashSet();
        }
    }
}
