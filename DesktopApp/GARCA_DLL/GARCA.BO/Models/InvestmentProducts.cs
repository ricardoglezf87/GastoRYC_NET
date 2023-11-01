using GARCA.DAO.Models;
using System.ComponentModel;

namespace GARCA.BO.Models
{
    public class InvestmentProducts : ModelBase
    {
        public virtual String? Description { set; get; }

        public virtual int? InvestmentProductsTypesid { set; get; }

        public virtual InvestmentProductsTypes? InvestmentProductsTypes { set; get; }

        public virtual String? Symbol { set; get; }

        public virtual String? Url { set; get; }

        [DefaultValue(true)]
        public virtual bool? Active { set; get; }

        internal InvestmentProductsDao ToDao()
        {
            return new InvestmentProductsDao
            {
                Id = Id,
                Description = Description,
                InvestmentProductsTypesid = InvestmentProductsTypesid,
                InvestmentProductsTypes = null,
                Symbol = Symbol,
                Url = Url,
                Active = Active
            };
        }

        public static explicit operator InvestmentProducts?(InvestmentProductsDao? v)
        {
            return v == null
                ? null
                : new InvestmentProducts
                {
                    Id = v.Id,
                    Description = v.Description,
                    InvestmentProductsTypesid = v.InvestmentProductsTypesid,
                    InvestmentProductsTypes = v.InvestmentProductsTypes != null ? (InvestmentProductsTypes?)v.InvestmentProductsTypes : null,
                    Symbol = v.Symbol,
                    Url = v.Url,
                    Active = v.Active
                };
        }
    }
}
