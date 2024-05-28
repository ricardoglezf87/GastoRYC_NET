using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class InvestmentProductsRepository : RepositoryBase<InvestmentProducts>
    {
        public async Task<IEnumerable<InvestmentProducts>?> GetAllOpened()
        {
            using (var connection = DBContext.OpenConnection())
            {
                return await connection.SelectAsync<InvestmentProducts, InvestmentProductsTypes, InvestmentProducts>(x => x.Active == true);
            }
        }
    }
}
