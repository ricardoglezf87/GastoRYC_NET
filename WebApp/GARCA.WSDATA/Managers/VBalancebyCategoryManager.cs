
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Managers
{
    public class VBalancebyCategoryManager
    {
        public async Task<IEnumerable<VBalancebyCategory>?> GetAll()
        {
            return await dbContext.OpenConnection().GetAllAsync<VBalancebyCategory>();
        }
    }
}
