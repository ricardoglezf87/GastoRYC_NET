using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("InvestmentProducts")]
    public class InvestmentProductsDAO : ModelBaseDAO
    {
        [Column("description")]
        public virtual String? description { set; get; }

        [Column("investmentProductsTypesid")]
        public virtual int? investmentProductsTypesid { set; get; }

        [Column("investmentProductsTypes")]
        public virtual InvestmentProductsTypesDAO? investmentProductsTypes { set; get; }

        [Column("symbol")]
        public virtual String? symbol { set; get; }

        [Column("url")]
        public virtual String? url { set; get; }

        [Column("active")]
        [DefaultValue(true)]
        public virtual bool? active { set; get; }
    }
}
