using GARCA.wsData.Repositories;
using GARCA.Models;
using static GARCA.Utils.Enums.EnumCategories;

namespace GARCA.Data.Services
{
    public class CategoriesTypesService : ServiceBase<CategoriesTypesRepository, CategoriesTypes>
    {
        public async Task<IEnumerable<CategoriesTypes>?> GetAllWithoutSpecialTransfer()
        {
            return (await GetAll())?.Where(x => x.Id is not (int)ECategoriesTypes.Specials and
                    not (int)ECategoriesTypes.Transfers);
        }
    }
}
