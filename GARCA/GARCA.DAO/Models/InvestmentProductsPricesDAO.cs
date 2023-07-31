using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("InvestmentProductsPrices")]
    public class InvestmentProductsPricesDAO : ModelBaseDAO
    {
        [Column("date")]
        public virtual DateTime? date { set; get; }

        [Column("investmentProductsid")]
        public virtual int? investmentProductsid { set; get; }

        [Column("investmentProducts")]
        public virtual InvestmentProductsDAO? investmentProducts { set; get; }

        [Column("prices")]
        public virtual Decimal? prices { set; get; }
    }
}
