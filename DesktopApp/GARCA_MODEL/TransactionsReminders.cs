using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("TransactionsReminders")]
    public class TransactionsReminders : ModelBase
    {
        [Column("periodsRemindersid")]
        public virtual int? PeriodsRemindersId { set; get; }

        [Column("periodsReminders")]
        public virtual PeriodsReminders? PeriodsReminders { set; get; }

        [Column("autoRegister")]
        public virtual bool? AutoRegister { set; get; }

        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("accountid")]
        public virtual int? AccountsId { set; get; }

        [Column("account")]
        public virtual Accounts? Accounts { set; get; }

        [Column("personid")]
        public virtual int? PersonsId { set; get; }

        [Column("person")]
        public virtual Persons? Persons { set; get; }

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

        [Column("splits")]
        public virtual HashSet<SplitsReminders>? Splits { set; get; }

        [Column("transactionStatusid")]
        public virtual int? TransactionsStatusId { set; get; }

        [Column("transactionStatus")]
        public virtual TransactionsStatus? TransactionsStatus { set; get; }

        [NotMapped]
        public virtual decimal? Amount => AmountIn - AmountOut;

    }
}
