using GARCA.DAO.Models;
using System;
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

        internal InvestmentProductsDAO ToDao()
        {
            return new InvestmentProductsDAO
            {
                id = Id,
                description = Description,
                investmentProductsTypesid = InvestmentProductsTypesid,
                investmentProductsTypes = null,
                symbol = Symbol,
                url = Url,
                active = Active
            };
        }

        public static explicit operator InvestmentProducts?(InvestmentProductsDAO? v)
        {
            return v == null
                ? null
                : new InvestmentProducts
                {
                    Id = v.id,
                    Description = v.description,
                    InvestmentProductsTypesid = v.investmentProductsTypesid,
                    InvestmentProductsTypes = v.investmentProductsTypes != null ? (InvestmentProductsTypes?)v.investmentProductsTypes : null,
                    Symbol = v.symbol,
                    Url = v.url,
                    Active = v.active
                };
        }
    }
}
