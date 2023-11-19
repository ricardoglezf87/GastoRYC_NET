using Dapper;

using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Data.Services;
using Microsoft.Data.Sqlite;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;
using GARCA_DATA.Managers;
using static GARCA.Data.Services.CategoriesTypesService;

namespace GARCA.Data.Services
{
    public class CategoriesService : ServiceBase<CategoriesManager, Categories>
    {
        public enum ESpecialCategories
        {
            Cierre = -2,
            Split = -1,
            WithoutCategory = 0
        }

        public async Task<bool> IsTranfer(int id)
        {
            return await iCategoriesTypesService.IsTranfer((await GetById(id))?.CategoriesTypesId ?? -99);
        }

        public async Task<IEnumerable<Categories>?> GetAllWithoutSpecialTransfer()
        {
            return (await GetAll())?.Where(x => !x.CategoriesTypesId.
                Equals((int)CategoriesTypesService.ECategoriesTypes.Transfers) &&
                !x.CategoriesTypesId.Equals((int)CategoriesTypesService.ECategoriesTypes.Specials));
        }

        public async Task<int> GetNextId()
        {
            return await manager.GetNextId();
        }
    }
}
