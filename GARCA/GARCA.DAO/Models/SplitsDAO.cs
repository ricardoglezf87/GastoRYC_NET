using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Splits")]
    public class SplitsDAO : ModelBaseDAO
    {
        [Column("transactionid")]
        public virtual int? transactionid { set; get; }

        [Column("transaction")]
        public virtual TransactionsDAO? transaction { set; get; }

        [Column("tagid")]
        public virtual int? tagid { set; get; }

        [Column("tag")]
        public virtual TagsDAO? tag { set; get; }

        [Column("categoryid")]
        public virtual int? categoryid { set; get; }

        [Column("category")]
        public virtual CategoriesDAO? category { set; get; }

        [Column("amountIn")]
        public virtual Decimal? amountIn { set; get; }

        [Column("amountOut")]
        public virtual Decimal? amountOut { set; get; }

        [Column("memo")]
        public virtual String? memo { set; get; }

        [Column("tranferid")]
        public virtual int? tranferid { set; get; }

    }
}
