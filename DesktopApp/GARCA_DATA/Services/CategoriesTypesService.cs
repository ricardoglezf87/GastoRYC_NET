using Dapper;
using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Data.Services;
using Microsoft.Data.Sqlite;
using static GARCA.Data.IOC.DependencyConfig;
using GARCA_DATA.Managers;

namespace GARCA.Data.Services
{
    public class CategoriesTypesService : ServiceBase<CategoriesTypesManager, CategoriesTypes, Int32>
    {
        public enum ECategoriesTypes
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        public async Task<IEnumerable<CategoriesTypes>?> GetAllWithoutSpecialTransfer()
        {
            return (await GetAll())?.Where(x => x.Id is not (int)ECategoriesTypes.Specials and
                    not (int)ECategoriesTypes.Transfers);
        }
    }
}
