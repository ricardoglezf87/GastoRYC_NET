
using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Data.Services;
using GARCA.Models;
using Dommel;

namespace GARCA.Data.Managers
{
    public class VBalancebyCategoryManager
    {
        public async Task<IEnumerable<VBalancebyCategory>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<VBalancebyCategory>();
        }
    }
}
