using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("InvestmentProducts")]
    public class InvestmentProducts : ModelBase
    {
        [Column("description")]
        public virtual String? Description { set; get; }

        [Column("investmentProductsTypesid")]
        public virtual int? InvestmentProductsTypesid { set; get; }

        [Column("investmentProductsTypes")]
        public virtual InvestmentProductsTypes? InvestmentProductsTypes { set; get; }

        [Column("symbol")]
        public virtual String? Symbol { set; get; }

        [Column("url")]
        public virtual String? Url { set; get; }

        [Column("active")]
        [DefaultValue(true)]
        public virtual bool? Active { set; get; }
    }
}
