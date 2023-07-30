using GARCA.Utlis.Extensions;

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
            investementProductsTypesManager = new InvestmentProductsTypesManager();
        }

        public HashSet<InvestmentProductsTypes?>? GetAll()
        {
            return investementProductsTypesManager.GetAll()?.ToHashSetBo();
        }

        public InvestmentProductsTypes? GetById(int? id)
        {
            return (InvestmentProductsTypes?)investementProductsTypesManager.GetById(id);
        }


    }
}
