using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class InvestmentProductsService : ServiceBase<InvestmentProductsManager, InvestmentProducts>
    {
        public async Task<IEnumerable<InvestmentProducts>?> GetAllOpened()
        {
            return await manager.GetAllOpened();
        }
    }
}
