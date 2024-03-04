using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Managers
{
    public class InvestmentProductsManager : ManagerBase<InvestmentProducts>
    {
        public async Task<IEnumerable<InvestmentProducts>?> GetAllOpened()
        {
            return await dbContext.OpenConnection().SelectAsync<InvestmentProducts, InvestmentProductsTypes, InvestmentProducts>(x => x.Active == true);
        }
    }
}
