using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("TransactionsReminders")]
    public class TransactionsRemindersDAO : ModelBaseDAO
    {
        [Column("periodsRemindersid")]
        public virtual int? periodsRemindersid { set; get; }

        [Column("periodsReminders")]
        public virtual PeriodsRemindersDAO? periodsReminders { set; get; }

        [Column("autoRegister")]
        public virtual bool? autoRegister { set; get; }

        [Column("date")]
        public virtual DateTime? date { set; get; }

        [Column("accountid")]
        public virtual int? accountid { set; get; }

        [Column("account")]
        public virtual AccountsDAO? account { set; get; }

        [Column("personid")]
        public virtual int? personid { set; get; }

        [Column("person")]
        public virtual PersonsDAO? person { set; get; }

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

        [Column("splits")]
        public virtual HashSet<SplitsRemindersDAO>? splits { set; get; }

        [Column("transactionStatusid")]
        public virtual int? transactionStatusid { set; get; }

        [Column("transactionStatus")]
        public virtual TransactionsStatusDAO? transactionStatus { set; get; }
    }
}
