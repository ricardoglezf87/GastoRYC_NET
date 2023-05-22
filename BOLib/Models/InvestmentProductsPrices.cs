using DAOLib.Models;
using System;

namespace BOLib.Models
{
    public class InvestmentProductsPrices : ModelBase
    {
        public virtual DateTime? date { set; get; }
        public virtual int? investmentProductsid { set; get; }
        public virtual InvestmentProducts? investmentProducts { set; get; }
        public virtual Decimal? prices { set; get; }

        internal InvestmentProductsPricesDAO? toDAO()
        {
            return new InvestmentProductsPricesDAO()
            {
                id = this.id,
                date = this.date,
                investmentProducts = null,
                investmentProductsid = this.investmentProductsid,
                prices = this.prices
            };
        }

        public static explicit operator InvestmentProductsPrices?(InvestmentProductsPricesDAO? v)
        {
            return v == null
                ? null
                : new InvestmentProductsPrices()
                {
                    id = v.id,
                    date = v.date,
                    investmentProducts = (v.investmentProducts != null) ? (InvestmentProducts?)v.investmentProducts : null,
                    investmentProductsid = v.investmentProductsid,
                    prices = v.prices
                };
        }
    }
}
