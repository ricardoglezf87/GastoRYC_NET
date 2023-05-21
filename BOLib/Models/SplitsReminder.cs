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
                transaction = null,
                categoryid = this.categoryid,
                category = null,
                amountOut = this.amountOut,
                amountIn = this.amountIn,
                memo = this.memo,
                tranferid = this.tranferid,
                tagid = this.tagid,
                tag = null
            };
        }

        public static explicit operator SplitsReminders?(SplitsRemindersDAO? v)
        {
            return v == null
                ? null
                : new SplitsReminders()
                {
                    id = v.id,
                    transactionid = v.transactionid,
                    transaction = (v.transaction != null) ? (TransactionsReminders?)v.transaction : null,
                    categoryid = v.categoryid,
                    category = (v.category != null) ? (Categories?)v.category : null,
                    amountOut = v.amountOut,
                    amountIn = v.amountIn,
                    memo = v.memo,
                    tranferid = v.tranferid,
                    tagid = v.tagid,
                    tag = (v.tag != null) ? (Tags?)v.tag : null
                };
        }
    }
}
