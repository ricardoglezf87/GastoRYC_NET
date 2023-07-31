using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("VBalancebyCategory")]
    public class VBalancebyCategoryDAO
    {
        [Column("year")]
        public virtual int? year { set; get; }

        [Column("month")]
        public virtual int? month { set; get; }

        [Column("categoriesTypesid")]
        public virtual int? categoriesTypesid { set; get; }

        [Column("categoryid")]
        public virtual int? categoryid { set; get; }

        [Column("category")]
        public virtual string? category { set; get; }

        [Column("amount")]
        public virtual Decimal? amount { set; get; }
    }
}
