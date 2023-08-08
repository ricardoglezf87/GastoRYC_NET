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


        internal SplitsDao ToDao()
        {
            return new SplitsDao
            {
                Id = Id,
                Transactionid = Transactionid,
                Transaction = null,
                Categoryid = Categoryid,
                Category = null,
                AmountOut = AmountOut,
                AmountIn = AmountIn,
                Memo = Memo,
                Tranferid = Tranferid,
                Tagid = Tagid,
                Tag = null
            };
        }

        public static explicit operator Splits?(SplitsDao? v)
        {
            return v == null
                ? null
                : new Splits
                {
                    Id = v.Id,
                    Transactionid = v.Transactionid,
                    Transaction = v.Transaction != null ? (Transactions?)v.Transaction : null,
                    Categoryid = v.Categoryid,
                    Category = v.Category != null ? (Categories?)v.Category : null,
                    AmountOut = v.AmountOut,
                    AmountIn = v.AmountIn,
                    Memo = v.Memo,
                    Tranferid = v.Tranferid,
                    Tagid = v.Tagid,
                    Tag = v.Tag != null ? (Tags?)v.Tag : null
                };
        }
    }
}
