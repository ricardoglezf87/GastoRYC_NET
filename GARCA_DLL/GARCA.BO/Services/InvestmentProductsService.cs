using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utlis.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GARCA.BO.Services
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
            return investmentProductsManager.GetAll()?.ToHashSetBo();
        }

        public InvestmentProducts? GetById(int? id)
        {
            return (InvestmentProducts?)investmentProductsManager.GetById(id);
        }

        public void Update(InvestmentProducts investmentProducts)
        {
            investmentProductsManager.Update(investmentProducts.ToDao());
        }

        public void Delete(InvestmentProducts investmentProducts)
        {
            investmentProductsManager.Delete(investmentProducts.ToDao());
        }

        public async Task<HashSet<InvestmentProducts?>?> GetAllOpened()
        {
            return await Task.Run(() => investmentProductsManager.GetAllOpened()?.ToHashSetBo());
        }
    }
}
