using DAOLib.Models;
using System;

namespace BOLib.Models
{
    public class SplitsReminders : ModelBase
    {
        public virtual int? transactionid { set; get; }

        public virtual TransactionsReminders? transaction { set; get; }

        public virtual int? tagid { set; get; }

        public virtual Tags? tag { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual Categories? category { set; get; }

        public virtual Decimal? amountIn { set; get; }

        public virtual Decimal? amountOut { set; get; }

        public virtual String? memo { set; get; }

        public virtual int? tranferid { set; get; }

        public SplitsRemindersDAO toDAO()
        {
            return new SplitsRemindersDAO()
            {
                id = this.id,
                transactionid = this.transactionid,
                transaction = this.transaction?.toDAO(),
                categoryid = this.categoryid,
                category = this.category?.toDAO(),
                amountOut = this.amountOut,
                amountIn = this.amountIn,
                memo = this.memo,
                tranferid = this.tranferid,
                tagid = this.tagid,
                tag = this.tag?.toDAO()
            };
        }

        public static explicit operator SplitsReminders(SplitsRemindersDAO v)
        {
            return new SplitsReminders()
            {
                id = v.id,
                transactionid = v.transactionid,
                transaction = (TransactionsReminders)v.transaction,
                categoryid = v.categoryid,
                category = (Categories)v.category,
                amountOut = v.amountOut,
                amountIn = v.amountIn,
                memo = v.memo,
                tranferid = v.tranferid,
                tagid = v.tagid,
                tag = (Tags)v.tag
            };
        }
    }
}
