using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("Splits")]
    public class Splits : ModelBase
    {
        [Column("transactionid")]
        public virtual int? TransactionsId { set; get; }

        [Column("transaction")]
        public virtual Transactions? Transactions { set; get; }

        [Column("tagid")]
        public virtual int? TagsId { set; get; }

        [Column("tag")]
        public virtual Tags? Tags { set; get; }

        [Column("categoryid")]
        public virtual int? CategoriesId { set; get; }

        [Column("category")]
        public virtual Categories? Categories { set; get; }

        [Column("amountIn")]
        public virtual Decimal? AmountIn { set; get; }

        [Column("amountOut")]
        public virtual Decimal? AmountOut { set; get; }

        [Column("memo")]
        public virtual String? Memo { set; get; }

        [Column("tranferid")]
        public virtual int? TranferId { set; get; }

        [NotMapped]
        public virtual decimal? Amount => AmountIn - AmountOut;

    }
}
