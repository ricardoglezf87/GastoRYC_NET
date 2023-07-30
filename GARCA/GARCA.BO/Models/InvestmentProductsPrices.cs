using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class InvestmentProductsPrices : ModelBase
    {
        public virtual DateTime? Date { set; get; }
        public virtual int? InvestmentProductsid { set; get; }
        public virtual InvestmentProducts? InvestmentProducts { set; get; }
        public virtual Decimal? Prices { set; get; }

        internal InvestmentProductsPricesDAO? ToDao()
        {
            return new InvestmentProductsPricesDAO
            {
                id = this.Id,
                date = this.Date,
                investmentProducts = null,
                investmentProductsid = this.InvestmentProductsid,
                prices = this.Prices
            };
        }

        public static explicit operator InvestmentProductsPrices?(InvestmentProductsPricesDAO? v)
        {
            return v == null
                ? null
                : new InvestmentProductsPrices
                {
                    Id = v.id,
                    Date = v.date,
                    InvestmentProducts = v.investmentProducts != null ? (InvestmentProducts?)v.investmentProducts : null,
                    InvestmentProductsid = v.investmentProductsid,
                    Prices = v.prices
                };
        }
    }
}
