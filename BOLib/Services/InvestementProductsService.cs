using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class InvestmentProductsService
    {
        private readonly InvestmentProductsManager investmentProductsManager;
        private static InvestmentProductsService? _instance;
        private static readonly object _lock = new();

        public static InvestmentProductsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new InvestmentProductsService();
                    }
                }
                return _instance;
            }
        }

        private InvestmentProductsService()
        {
            investmentProductsManager = new();
        }

        public List<InvestmentProducts?>? getAll()
        {
            return investmentProductsManager.getAll()?.toListBO();
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
    }
}
