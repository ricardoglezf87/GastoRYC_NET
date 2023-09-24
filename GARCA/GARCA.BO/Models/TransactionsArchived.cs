using GARCA.DAO.Models;
using GARCA.Utlis.Extensions;
using System;
using System.Collections.Generic;

namespace GARCA.BO.Models
{
    public class TransactionsArchived : ModelBase
    {
        public virtual DateTime? Date { set; get; }

        public virtual int? Accountid { set; get; }

        public virtual Accounts? Account { set; get; }

        public virtual int? Personid { set; get; }

        public virtual Persons? Person { set; get; }

        public virtual int? Tagid { set; get; }

        public virtual Tags? Tag { set; get; }

        public virtual int? Categoryid { set; get; }

        public virtual Categories? Category { set; get; }

        public virtual Decimal? AmountIn { set; get; }

        public virtual Decimal? AmountOut { set; get; }

        public virtual int? Tranferid { set; get; }

        public virtual int? TranferSplitid { set; get; }

        public virtual String? Memo { set; get; }

        public virtual int? TransactionStatusid { set; get; }

        public virtual TransactionsStatus? TransactionStatus { set; get; }

        public virtual int? InvestmentProductsid { set; get; }

        public virtual InvestmentProducts? InvestmentProducts { set; get; }

        public virtual HashSet<SplitsArchived?>? Splits { set; get; }

        public virtual Decimal? NumShares { set; get; }

        public virtual Decimal? PricesShares { set; get; }

        public virtual bool? InvestmentCategory { set; get; }

        public virtual String CategoryDescripGrid => InvestmentCategory.HasValue && InvestmentCategory.Value == false ?
            NumShares > 0 ? "Inversiones:Venta" : "Inversiones:Compra" : Category?.Description ?? String.Empty;

        public virtual String PersonDescripGrid => InvestmentCategory.HasValue && InvestmentCategory.Value == false ?
            InvestmentProducts?.Description ?? String.Empty : Person?.Name ?? String.Empty;

        public virtual Decimal? Amount => InvestmentCategory.HasValue && InvestmentCategory.Value == false ? Math.Round((NumShares ?? 0) * (PricesShares ?? 0), 2) : AmountIn - AmountOut;

        public virtual Double? Orden { set; get; }

        public virtual Decimal? Balance { set; get; }

        internal TransactionsArchivedDao ToDao()
        {
            return new TransactionsArchivedDao
            {
                Id = Id,
                Date = Date,
                Accountid = Accountid,
                Account = null,
                Personid = Personid,
                Person = null,
                Categoryid = Categoryid,
                Category = null,
                AmountIn = AmountIn,
                AmountOut = AmountOut,
                Memo = Memo,
                InvestmentCategory = InvestmentCategory,
                InvestmentProducts = null,
                InvestmentProductsid = InvestmentProductsid,
                Tranferid = Tranferid,
                TranferSplitid = TranferSplitid,
                TransactionStatus = null,
                TransactionStatusid = TransactionStatusid,
                NumShares = NumShares,
                PricesShares = PricesShares,
                Tagid = Tagid,
                Tag = null,
                Balance = Balance,
                Orden = Orden
            };
        }


        public static explicit operator TransactionsArchived?(TransactionsArchivedDao? v)
        {
            return v == null
                ? null
                : new TransactionsArchived
                {
                    Id = v.Id,
                    Date = v.Date,
                    Accountid = v.Accountid,
                    Account = v.Account != null ? (Accounts?)v.Account : null,
                    Personid = v.Personid,
                    Person = v.Person != null ? (Persons?)v.Person : null,
                    Categoryid = v.Categoryid,
                    Category = v.Category != null ? (Categories?)v.Category : null,
                    AmountIn = v.AmountIn,
                    AmountOut = v.AmountOut,
                    Memo = v.Memo,
                    InvestmentCategory = v.InvestmentCategory,
                    InvestmentProducts = v.InvestmentProducts != null ? (InvestmentProducts?)v.InvestmentProducts : null,
                    InvestmentProductsid = v.InvestmentProductsid,
                    Tranferid = v.Tranferid,
                    TranferSplitid = v.TranferSplitid,
                    TransactionStatus = v.TransactionStatus != null ? (TransactionsStatus?)v.TransactionStatus : null,
                    TransactionStatusid = v.TransactionStatusid,
                    NumShares = v.NumShares,
                    PricesShares = v.PricesShares,
                    Tagid = v.Tagid,
                    Tag = v.Tag != null ? (Tags?)v.Tag : null,
                    Balance = v.Balance,
                    Orden = v.Orden
                };
        }

        public override int CompareTo(object? obj)
        {
            return obj == null
                ? 1
                : obj is not Transactions otherTransaction
                ? throw new ArgumentException("El objeto proporcionado no es de tipo Transactions.")
                : otherTransaction.Orden.CompareTo(Orden);
        }
    }
}
