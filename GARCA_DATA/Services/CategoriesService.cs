using GARCA.wsData.Repositories;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;
using static GARCA.Utils.Enums.EnumCategories;

namespace GARCA.Data.Services
{
    public class CategoriesService : ServiceBase<CategoriesRepository, Categories>
    {
        public async Task<IEnumerable<Categories>?> GetAllWithoutSpecialTransfer()
        {
            return (await GetAll())?.Where(x => !x.CategoriesTypesId.
                Equals((int)ECategoriesTypes.Transfers) &&
                !x.CategoriesTypesId.Equals((int)ECategoriesTypes.Specials));
        }
    }
}
