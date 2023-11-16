using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("TransactionsReminders")]
    public class TransactionsReminders : ModelBase<Int32>
    {
        [Column("periodsRemindersid")]
        public virtual int? PeriodsRemindersid { set; get; }

        [Column("periodsReminders")]
        public virtual PeriodsReminders? PeriodsReminders { set; get; }

        [Column("autoRegister")]
        public virtual bool? AutoRegister { set; get; }

        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("accountid")]
        public virtual int? Accountid { set; get; }

        [Column("account")]
        public virtual Accounts? Account { set; get; }

        [Column("personid")]
        public virtual int? Personid { set; get; }

        [Column("person")]
        public virtual Persons? Person { set; get; }

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

        [Column("splits")]
        public virtual HashSet<SplitsReminders>? Splits { set; get; }

        [Column("transactionStatusid")]
        public virtual int? TransactionStatusid { set; get; }

        [Column("transactionStatus")]
        public virtual TransactionsStatus? TransactionStatus { set; get; }

        [NotMapped]
        public virtual decimal? Amount => AmountIn - AmountOut;

    }
}
