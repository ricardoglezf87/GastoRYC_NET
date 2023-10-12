using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("VBalancebyCategory")]
    public class VBalancebyCategoryDao
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
    }
}
