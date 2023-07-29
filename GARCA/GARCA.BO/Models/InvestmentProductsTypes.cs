using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class InvestmentProductsTypes : ModelBase
    {
        public virtual String? description { set; get; }
        
        public static explicit operator InvestmentProductsTypes?(InvestmentProductsTypesDAO? v)
        {
            return v == null
                ? null
                : new InvestmentProductsTypes
                {
                    id = v.id,
                    description = v.description
                };
        }
    }
}
