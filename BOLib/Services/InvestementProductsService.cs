using BOLib.Extensions;
using BOLib.Helpers;
using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class InvestmentProductsService
    {
        private readonly InvestmentProductsManager investmentProductsManager;

        public InvestmentProductsService()
        {
            investmentProductsManager = new();
        }

        public List<InvestmentProducts>? getAll()
        {
            return investmentProductsManager.getAll()?.toListBO();
        }

        public InvestmentProducts? getByID(int? id)
        {
            return (InvestmentProducts)investmentProductsManager.getByID(id);
        }

        public void update(InvestmentProducts investmentProducts)
        {
            investmentProductsManager.update(investmentProducts.toDAO());
        }

        public void delete(InvestmentProducts investmentProducts)
        {
            investmentProductsManager.delete(investmentProducts.toDAO());
        }
    }
}
