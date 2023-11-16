using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("SplitsArchived")]
    public class SplitsArchived : ModelBase<Int32>
    {
        [Column("idOriginal")]
        public virtual int? IdOriginal { get; set; }

        [Column("transactionid")]
        public virtual int? Transactionid { set; get; }

        [Column("transaction")]
        public virtual TransactionsArchived? Transaction { set; get; }

        [Column("tagid")]
        public virtual int? Tagid { set; get; }

        [Column("tag")]
        public virtual Tags? Tag { set; get; }

        [Column("categoryid")]
        public virtual int? Categoryid { set; get; }

        [Column("category")]
        public virtual Categories? Category { set; get; }

        [Column("amountIn")]
        public virtual Decimal? AmountIn { set; get; }

        [Column("amountOut")]
        public virtual Decimal? AmountOut { set; get; }

        [Column("memo")]
        public virtual String? Memo { set; get; }

        [Column("tranferid")]
        public virtual int? Tranferid { set; get; }

        [NotMapped]
        public virtual decimal? Amount => AmountIn - AmountOut;

    }
}
