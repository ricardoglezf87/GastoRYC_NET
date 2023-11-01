using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("VBalancebyCategory")]
    public class VBalancebyCategory
    {
        [Column("year")]
        public virtual int? Year { set; get; }

        [Column("month")]
        public virtual int? Month { set; get; }

        [Column("categoriesTypesid")]
        public virtual int? CategoriesTypesid { set; get; }

        [Column("categoryid")]
        public virtual int? Categoryid { set; get; }

        [Column("category")]
        public virtual string? Category { set; get; }

        [Column("amount")]
        public virtual Decimal? Amount { set; get; }

        [NotMapped]
        public virtual decimal? NegAmount => -Amount;
    }
}
