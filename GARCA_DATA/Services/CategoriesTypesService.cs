using GARCA.Data.Managers;
using GARCA.Models;
using static GARCA.Utils.Enums.EnumCategories;

namespace GARCA.Data.Services
{
    public class CategoriesTypesService : ServiceBase<CategoriesTypesManager, CategoriesTypes>
    {
        public async Task<bool> IsTranfer(int id)
        {
            return (await GetById(id))?.Id == (int)ECategoriesTypes.Transfers;
        }

        public async Task<IEnumerable<CategoriesTypes>?> GetAllWithoutSpecialTransfer()
        {
            return (await GetAll())?.Where(x => x.Id is not (int)ECategoriesTypes.Specials and
                    not (int)ECategoriesTypes.Transfers);
        }
    }
}
