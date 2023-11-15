using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class InvestmentProductsService : ServiceBase<InvestmentProductsManager, InvestmentProducts, Int32>
    {
        public void Update(InvestmentProducts investmentProducts)
        {
            manager.Update(investmentProducts);
        }

        public async Task<HashSet<InvestmentProducts>?> GetAllOpened()
        {
            return await Task.Run(() => manager.GetAllOpened()?.ToHashSet());
        }
    }
}
