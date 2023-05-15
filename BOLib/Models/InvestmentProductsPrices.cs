using DAOLib.Models;
using System;
using System.ComponentModel.DataAnnotations;

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
                investmentProducts = this.investmentProducts?.toDAO(),
                investmentProductsid = this.investmentProductsid,
                prices = this.prices
            };
        }

        public static explicit operator InvestmentProductsPrices?(InvestmentProductsPricesDAO v)
        {
            if (v == null) return null;

            return new InvestmentProductsPrices()
            {
                id = v.id,
                date = v.date,
                investmentProducts = (InvestmentProducts)v.investmentProducts,
                investmentProductsid = v.investmentProductsid,
                prices = v.prices
            };
        }
    }
}
