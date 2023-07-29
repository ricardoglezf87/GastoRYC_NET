using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
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

        public HashSet<InvestmentProductsTypes?>? getAll()
        {
            return investementProductsTypesManager.getAll()?.toHashSetBO();
        }

        public InvestmentProductsTypes? getByID(int? id)
        {
            return (InvestmentProductsTypes?)investementProductsTypesManager.getByID(id);
        }


    }
}
