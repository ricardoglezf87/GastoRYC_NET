using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class Splits : ModelBase
    {
        public virtual int? Transactionid { set; get; }

        public virtual Transactions? Transaction { set; get; }

        public virtual int? Tagid { set; get; }

        public virtual Tags? Tag { set; get; }

        public virtual int? Categoryid { set; get; }

        public virtual Categories? Category { set; get; }

        public virtual Decimal? AmountIn { set; get; }

        public virtual Decimal? AmountOut { set; get; }

        public virtual String? Memo { set; get; }

        public virtual int? Tranferid { set; get; }

        public virtual Decimal? Amount => AmountIn - AmountOut;


        internal SplitsDAO ToDao()
        {
            return new SplitsDAO
            {
                id = Id,
                transactionid = Transactionid,
                transaction = null,
                categoryid = Categoryid,
                category = null,
                amountOut = AmountOut,
                amountIn = AmountIn,
                memo = Memo,
                tranferid = Tranferid,
                tagid = Tagid,
                tag = null
            };
        }

        public static explicit operator Splits?(SplitsDAO? v)
        {
            return v == null
                ? null
                : new Splits
                {
                    Id = v.id,
                    Transactionid = v.transactionid,
                    Transaction = v.transaction != null ? (Transactions?)v.transaction : null,
                    Categoryid = v.categoryid,
                    Category = v.category != null ? (Categories?)v.category : null,
                    AmountOut = v.amountOut,
                    AmountIn = v.amountIn,
                    Memo = v.memo,
                    Tranferid = v.tranferid,
                    Tagid = v.tagid,
                    Tag = v.tag != null ? (Tags?)v.tag : null
                };
        }
    }
}
