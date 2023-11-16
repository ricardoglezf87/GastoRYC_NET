using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class InvestmentProductsService : ServiceBase<InvestmentProductsManager, InvestmentProducts, Int32>
    {
        public async Task<IEnumerable<InvestmentProducts>?> GetAllOpened()
        {
            return await manager.GetAllOpened();
        }
    }
}
