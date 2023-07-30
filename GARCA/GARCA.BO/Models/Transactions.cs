using GARCA.DAO.Models;
using System;
using System.Collections.Generic;
using GARCA.Utlis.Extensions;

namespace GARCA.BO.Models
{
    public class Transactions : ModelBase
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

        public virtual HashSet<Splits?>? Splits { set; get; }

        public virtual Decimal? NumShares { set; get; }

        public virtual Decimal? PricesShares { set; get; }

        public virtual bool? InvestmentCategory { set; get; }

        public virtual String? CategoryDescripGrid => InvestmentCategory.HasValue && InvestmentCategory.Value == false ?
            NumShares > 0 ? "Inversiones:Venta" : "Inversiones:Compra" : Category?.Description ?? String.Empty;

        public virtual String? PersonDescripGrid => InvestmentCategory.HasValue && InvestmentCategory.Value == false ?
            InvestmentProducts?.Description ?? String.Empty : Person?.Name ?? String.Empty;

        public virtual Decimal? Amount => InvestmentCategory.HasValue && InvestmentCategory.Value == false ? Math.Round((NumShares ?? 0) * (PricesShares ?? 0), 2) : AmountIn - AmountOut;

        public virtual Double? Orden { set; get; }

        public virtual Decimal? Balance { set; get; }

        

        internal TransactionsDAO ToDao()
        {
            return new TransactionsDAO
            {
                id = this.Id,
                date = this.Date,
                accountid = this.Accountid,
                account = null,
                personid = this.Personid,
                person = null,
                categoryid = this.Categoryid,
                category = null,
                amountIn = this.AmountIn,
                amountOut = this.AmountOut,
                memo = this.Memo,
                investmentCategory = this.InvestmentCategory,
                investmentProducts = null,
                investmentProductsid = this.InvestmentProductsid,
                tranferid = this.Tranferid,
                tranferSplitid = this.TranferSplitid,
                transactionStatus = null,
                transactionStatusid = this.TransactionStatusid,
                numShares = this.NumShares,
                pricesShares = this.PricesShares,
                tagid = this.Tagid,
                tag = null,
                balance = this.Balance,
                orden = this.Orden
            };
        }


        public static explicit operator Transactions?(TransactionsDAO? v)
        {
            return v == null
                ? null
                : new Transactions
                {
                    Id = v.id,
                    Date = v.date,
                    Accountid = v.accountid,
                    Account = v.account != null ? (Accounts?)v.account : null,
                    Personid = v.personid,
                    Person = v.person != null ? (Persons?)v.person : null,
                    Categoryid = v.categoryid,
                    Category = v.category != null ? (Categories?)v.category : null,
                    AmountIn = v.amountIn,
                    AmountOut = v.amountOut,
                    Memo = v.memo,
                    InvestmentCategory = v.investmentCategory,
                    InvestmentProducts = v.investmentProducts != null ? (InvestmentProducts?)v.investmentProducts : null,
                    InvestmentProductsid = v.investmentProductsid,
                    Tranferid = v.tranferid,
                    TranferSplitid = v.tranferSplitid,
                    TransactionStatus = v.transactionStatus != null ? (TransactionsStatus?)v.transactionStatus : null,
                    TransactionStatusid = v.transactionStatusid,
                    NumShares = v.numShares,
                    PricesShares = v.pricesShares,
                    Tagid = v.tagid,
                    Tag = v.tag != null ? (Tags?)v.tag : null,
                    Balance = v.balance,
                    Orden = v.orden
                };
        }

        public override int CompareTo(object? obj)
        {
            if (obj == null)
            {
                return 1; 
            }

            if (obj is not Transactions otherTransaction)
            {
                throw new ArgumentException("El objeto proporcionado no es de tipo Transactions.");
            }

            return otherTransaction.Orden.CompareTo(this.Orden);
        }
    }
}
