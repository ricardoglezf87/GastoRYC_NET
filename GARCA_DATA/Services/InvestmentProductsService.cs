using GARCA.wsData.Repositories;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class InvestmentProductsService : ServiceBase<InvestmentProductsRepository, InvestmentProducts>
    {
        public async Task<IEnumerable<InvestmentProducts>?> GetAllOpened()
        {
            return await repository.GetAllOpened();
        }
    }
}
