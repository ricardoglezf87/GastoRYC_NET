using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class InvestmentProductsRepository : RepositoryBase<InvestmentProducts>
    {
        public async Task<IEnumerable<InvestmentProducts>?> GetAllOpened()
        {
            return await dbContext.OpenConnection().SelectAsync<InvestmentProducts, InvestmentProductsTypes, InvestmentProducts>(x => x.Active == true);
        }
    }
}
