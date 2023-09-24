using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("SplitsArchived")]
    public class SplitsArchivedDao : ModelBaseDao
    {
        [Column("transactionid")]
        public virtual int? Transactionid { set; get; }

        [Column("transaction")]
        public virtual TransactionsArchivedDao? Transaction { set; get; }

        [Column("tagid")]
        public virtual int? Tagid { set; get; }

        [Column("tag")]
        public virtual TagsDao? Tag { set; get; }

        [Column("categoryid")]
        public virtual int? Categoryid { set; get; }

        [Column("category")]
        public virtual CategoriesDao? Category { set; get; }

        [Column("amountIn")]
        public virtual Decimal? AmountIn { set; get; }

        [Column("amountOut")]
        public virtual Decimal? AmountOut { set; get; }

        [Column("memo")]
        public virtual String? Memo { set; get; }

        [Column("tranferid")]
        public virtual int? Tranferid { set; get; }

    }
}
