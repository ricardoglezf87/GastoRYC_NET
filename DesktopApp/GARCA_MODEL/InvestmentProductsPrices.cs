using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("InvestmentProductsPrices")]
    public class InvestmentProductsPrices : ModelBase<Int32>
    {
        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("investmentProductsid")]
        public virtual int? InvestmentProductsid { set; get; }

        [Column("investmentProducts")]
        public virtual InvestmentProducts? InvestmentProducts { set; get; }

        [Column("prices")]
        public virtual Decimal? Prices { set; get; }
    }
}
