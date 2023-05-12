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

        public static explicit operator InvestmentProductsPrices(InvestmentProductsPricesDAO v)
        {
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
