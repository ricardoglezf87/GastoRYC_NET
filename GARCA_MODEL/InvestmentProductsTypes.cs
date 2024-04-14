using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("InvestmentProductsTypes")]
    public class InvestmentProductsTypes : ModelBase
    {
        [Column("description")]
        public virtual String? Description { set; get; }
    }
}
