using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("InvestmentProductsPrices")]
    public class InvestmentProductsPricesDao : ModelBaseDao
    {
        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("investmentProductsid")]
        public virtual int? InvestmentProductsid { set; get; }

        [Column("investmentProducts")]
        public virtual InvestmentProductsDao? InvestmentProducts { set; get; }

        [Column("prices")]
        public virtual Decimal? Prices { set; get; }
    }
}
