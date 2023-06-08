using DAOLib.Models;
using System;
using System.ComponentModel;

namespace BOLib.Models
{
    public class InvestmentProducts : ModelBase
    {
        public virtual String? description { set; get; }

        public virtual int? investmentProductsTypesid { set; get; }

        public virtual InvestmentProductsTypes? investmentProductsTypes { set; get; }

        public virtual String? symbol { set; get; }

        public virtual String? url { set; get; }

        [DefaultValue(true)]
        public virtual bool? active { set; get; }

        internal InvestmentProductsDAO toDAO()
        {
            return new InvestmentProductsDAO()
            {
                id = this.id,
                description = this.description,
                investmentProductsTypesid = this.investmentProductsTypesid,
                investmentProductsTypes = null,
                symbol = this.symbol,
                url = this.url,
                active = this.active
            };
        }

        public static explicit operator InvestmentProducts?(InvestmentProductsDAO? v)
        {
            return v == null
                ? null
                : new InvestmentProducts()
                {
                    id = v.id,
                    description = v.description,
                    investmentProductsTypesid = v.investmentProductsTypesid,
                    investmentProductsTypes = (v.investmentProductsTypes != null) ? (InvestmentProductsTypes?)v.investmentProductsTypes : null,                    
                    symbol = v.symbol,
                    url = v.url,
                    active = v.active
                };
        }
    }
}
