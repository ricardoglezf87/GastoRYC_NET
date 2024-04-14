
using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class VBalancebyCategoryManager
    {
        public async Task<IEnumerable<VBalancebyCategory>?> GetAll()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.GetAllAsync<VBalancebyCategory>();
            }
        }
    }
}
