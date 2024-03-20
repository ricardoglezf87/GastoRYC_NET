
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class VBalancebyCategoryRepository
    {
        public async Task<IEnumerable<VBalancebyCategory>?> GetAll()
        {
            return await dbContext.OpenConnection().GetAllAsync<VBalancebyCategory>();
        }
    }
}
