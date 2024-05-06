using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class InvestmentProductsManager : ManagerBase<InvestmentProducts>
    {
        public async Task<IEnumerable<InvestmentProducts>?> GetAllOpened()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<InvestmentProducts, InvestmentProductsTypes, InvestmentProducts>(x => x.Active == true);
            }
        }
    }
}
