using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("InvestmentProducts")]
    public class InvestmentProductsDao : ModelBaseDao
    {
        [Column("description")]
        public virtual String? Description { set; get; }

        [Column("investmentProductsTypesid")]
        public virtual int? InvestmentProductsTypesid { set; get; }

        [Column("investmentProductsTypes")]
        public virtual InvestmentProductsTypesDao? InvestmentProductsTypes { set; get; }

        [Column("symbol")]
        public virtual String? Symbol { set; get; }

        [Column("url")]
        public virtual String? Url { set; get; }

        [Column("active")]
        [DefaultValue(true)]
        public virtual bool? Active { set; get; }
    }
}
