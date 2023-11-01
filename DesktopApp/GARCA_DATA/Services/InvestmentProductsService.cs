using GARCA.Models;
using GARCA.Data.Managers;


namespace GARCA.Data.Services
{
    public class InvestmentProductsService
    {
        private readonly InvestmentProductsManager investmentProductsManager;

        public InvestmentProductsService()
        {
            investmentProductsManager = new InvestmentProductsManager();
        }

        public HashSet<InvestmentProducts?>? GetAll()
        {
            return investmentProductsManager.GetAll()?.ToHashSet();
        }

        public InvestmentProducts? GetById(int? id)
        {
            return (InvestmentProducts)investmentProductsManager.GetById(id);
        }

        public void Update(InvestmentProducts investmentProducts)
        {
            investmentProductsManager.Update(investmentProducts);
        }

        public void Delete(InvestmentProducts investmentProducts)
        {
            investmentProductsManager.Delete(investmentProducts);
        }

        public async Task<HashSet<InvestmentProducts?>?> GetAllOpened()
        {
            return await Task.Run(() => investmentProductsManager.GetAllOpened()?.ToHashSet());
        }
    }
}
