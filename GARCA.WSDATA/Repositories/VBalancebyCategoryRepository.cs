
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class VBalancebyCategoryRepository
    {
        public async Task<IEnumerable<VBalancebyCategory>?> GetAll()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.GetAllAsync<VBalancebyCategory>();
            }
        }
    }
}
