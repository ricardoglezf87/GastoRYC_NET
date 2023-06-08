using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class InvestmentProductsTypesService
    {
        private readonly InvestmentProductsTypesManager investementProductsTypesManager;
        private static InvestmentProductsTypesService? _instance;
        private static readonly object _lock = new();

        public static InvestmentProductsTypesService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new InvestmentProductsTypesService();
                    }
                }
                return _instance;
            }
        }

        private InvestmentProductsTypesService()
        {
            investementProductsTypesManager = new();
        }

        public List<InvestmentProductsTypes?>? getAll()
        {
            return investementProductsTypesManager.getAll()?.toListBO();
        }

        public InvestmentProductsTypes? getByID(int? id)
        {
            return (InvestmentProductsTypes?)investementProductsTypesManager.getByID(id);
        }


    }
}
