using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class InvestmentProductsTypesService
    {
        private readonly InvestmentProductsTypesManager investementProductsTypesManager;

        public InvestmentProductsTypesService()
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
