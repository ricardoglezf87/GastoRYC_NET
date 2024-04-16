using GARCA.Data.Managers;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;
using static GARCA.Utils.Enums.EnumCategories;

namespace GARCA.Data.Services
{
    public class CategoriesService : ServiceBase<CategoriesManager, Categories>
    {
        public async Task<bool> IsTranfer(int id)
        {
            return await iCategoriesTypesService.IsTranfer((await GetById(id))?.CategoriesTypesId ?? -99);
        }

        public async Task<IEnumerable<Categories>?> GetAllWithoutSpecialTransfer()
        {
            return (await GetAll())?.Where(x => !x.CategoriesTypesId.
                Equals((int)ECategoriesTypes.Transfers) &&
                !x.CategoriesTypesId.Equals((int)ECategoriesTypes.Specials));
        }

        public async Task<int> GetNextId()
        {
            return await manager.GetNextId();
        }
    }
}
