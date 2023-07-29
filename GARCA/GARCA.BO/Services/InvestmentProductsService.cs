using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;
  
namespace GARCA.BO.Services
{
    public class InvestmentProductsService
    {
        private readonly InvestmentProductsManager investmentProductsManager;

        public InvestmentProductsService()
        {
            investmentProductsManager = new();
        }

        public HashSet<InvestmentProducts?>? getAll()
        {
            return investmentProductsManager.getAll()?.toHashSetBO();
        }

        public InvestmentProducts? getByID(int? id)
        {
            return (InvestmentProducts?)investmentProductsManager.getByID(id);
        }

        public void update(InvestmentProducts investmentProducts)
        {
            investmentProductsManager.update(investmentProducts?.toDAO());
        }

        public void delete(InvestmentProducts investmentProducts)
        {
            investmentProductsManager.delete(investmentProducts?.toDAO());
        }

        public async Task<List<InvestmentProducts?>?> getAllOpened()
        {
            return await Task.Run(() => investmentProductsManager.getAllOpened()?.toListBO());
        }
    }
}
