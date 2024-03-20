
using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

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
