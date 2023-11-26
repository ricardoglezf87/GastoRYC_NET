using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class InvestmentProductsManager : ManagerBase<InvestmentProducts>
    {
        public async Task<IEnumerable<InvestmentProducts>?> GetAllOpened()
        {
            return await iRycContextService.getConnection().SelectAsync<InvestmentProducts, InvestmentProductsTypes, InvestmentProducts>(x => x.Active == true);
        }
    }
}
