using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("TransactionsReminders")]
    public class TransactionsRemindersDao : ModelBaseDao
    {
        [Column("periodsRemindersid")]
        public virtual int? PeriodsRemindersid { set; get; }

        [Column("periodsReminders")]
        public virtual PeriodsRemindersDao? PeriodsReminders { set; get; }

        [Column("autoRegister")]
        public virtual bool? AutoRegister { set; get; }

        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("accountid")]
        public virtual int? Accountid { set; get; }

        [Column("account")]
        public virtual AccountsDao? Account { set; get; }

        [Column("personid")]
        public virtual int? Personid { set; get; }

        [Column("person")]
        public virtual PersonsDao? Person { set; get; }

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

        [Column("splits")]
        public virtual HashSet<SplitsRemindersDao>? Splits { set; get; }

        [Column("transactionStatusid")]
        public virtual int? TransactionStatusid { set; get; }

        [Column("transactionStatus")]
        public virtual TransactionsStatusDao? TransactionStatus { set; get; }
    }
}
