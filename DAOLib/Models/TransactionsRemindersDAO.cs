using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("TransactionsReminders")]
    public class TransactionsRemindersDAO : IModelDAO
    {
        public virtual int? periodsRemindersid { set; get; }
        public virtual PeriodsRemindersDAO? periodsReminders { set; get; }

        public virtual bool? autoRegister { set; get; }

        public virtual DateTime? date { set; get; }

        public virtual int? accountid { set; get; }

        public virtual AccountsDAO? account { set; get; }

        public virtual int? personid { set; get; }

        public virtual PersonsDAO? person { set; get; }

        public virtual int? tagid { set; get; }

        public virtual TagsDAO? tag { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual CategoriesDAO? category { set; get; }

        public virtual Decimal? amountIn { set; get; }

        public virtual Decimal? amountOut { set; get; }

        public virtual int? tranferid { set; get; }

        public virtual int? tranferSplitid { set; get; }

        public virtual String? memo { set; get; }

        public virtual int? transactionStatusid { set; get; }

        public virtual TransactionsStatusDAO? transactionStatus { set; get; }

        public virtual List<SplitsRemindersDAO>? splits { set; get; }        
    }
}
