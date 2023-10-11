using GARCA.DAO.Models;

namespace GARCA.BO.Models
{
    public class InvestmentProductsPrices : ModelBase
    {
        public virtual DateTime? Date { set; get; }
        public virtual int? InvestmentProductsid { set; get; }
        public virtual InvestmentProducts? InvestmentProducts { set; get; }
        public virtual Decimal? Prices { set; get; }

        internal InvestmentProductsPricesDao ToDao()
        {
            return new InvestmentProductsPricesDao
            {
                Id = Id,
                Date = Date,
                InvestmentProducts = null,
                InvestmentProductsid = InvestmentProductsid,
                Prices = Prices
            };
        }

        public static explicit operator InvestmentProductsPrices?(InvestmentProductsPricesDao? v)
        {
            return v == null
                ? null
                : new InvestmentProductsPrices
                {
                    Id = v.Id,
                    Date = v.Date,
                    InvestmentProducts = v.InvestmentProducts != null ? (InvestmentProducts?)v.InvestmentProducts : null,
                    InvestmentProductsid = v.InvestmentProductsid,
                    Prices = v.Prices
                };
        }
    }
}
