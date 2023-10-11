using GARCA.DAO.Models;

namespace GARCA.BO.Models
{
    public class InvestmentProductsTypes : ModelBase
    {
        public virtual String? Description { set; get; }

        public static explicit operator InvestmentProductsTypes?(InvestmentProductsTypesDao? v)
        {
            return v == null
                ? null
                : new InvestmentProductsTypes
                {
                    Id = v.Id,
                    Description = v.Description
                };
        }
    }
}
