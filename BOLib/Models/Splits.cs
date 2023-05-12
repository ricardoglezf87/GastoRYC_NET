using DAOLib.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace BOLib.Models
{
    public class Splits : ModelBase
    {
        public virtual int? transactionid { set; get; }

        public virtual Transactions? transaction { set; get; }

        public virtual int? tagid { set; get; }

        public virtual Tags? tag { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual Categories? category { set; get; }

        public virtual Decimal? amountIn { set; get; }

        public virtual Decimal? amountOut { set; get; }

        public virtual String? memo { set; get; }

        public virtual int? tranferid { set; get; }

        internal SplitsDAO toDAO()
        {
            return new SplitsDAO()
            {
                id = this.id,
                transactionid = this.transactionid,
                transaction = this.transaction.toDAO(),
                categoryid = this.categoryid,
                category = this.category.toDAO(),
                amountOut = this.amountOut,
                amountIn = this.amountIn,
                memo = this.memo,
                tranferid = this.tranferid,
                tagid = this.tagid,
                tag = this.tag.toDAO()
            };
        }

        public static explicit operator Splits(SplitsDAO v)
        {
            return new Splits()
            {
                id = v.id,
                transactionid = v.transactionid,
                transaction = (Transactions)v.transaction,
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
