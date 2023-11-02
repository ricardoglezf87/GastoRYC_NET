using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class InvestmentProductsTypesService
    {
        private readonly InvestmentProductsTypesManager investementProductsTypesManager;

        public InvestmentProductsTypesService()
        {
            investementProductsTypesManager = new InvestmentProductsTypesManager();
        }

        public HashSet<InvestmentProductsTypes?>? GetAll()
        {
            return investementProductsTypesManager.GetAll()?.ToHashSet();
        }

        public InvestmentProductsTypes? GetById(int? id)
        {
            return investementProductsTypesManager.GetById(id);
        }


    }
}
