using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("InvestmentProductsPrices")]
    public class InvestmentProductsPricesDAO : ModelBaseDAO
    {
        public virtual DateTime? date { set; get; }
        public virtual int? investmentProductsid { set; get; }
        public virtual InvestmentProductsDAO? investmentProducts { set; get; }
        public virtual Decimal? prices { set; get; }
    }
}
