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
                id = this.Id,
                description = this.Description,
                investmentProductsTypesid = this.InvestmentProductsTypesid,
                investmentProductsTypes = null,
                symbol = this.Symbol,
                url = this.Url,
                active = this.Active
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
