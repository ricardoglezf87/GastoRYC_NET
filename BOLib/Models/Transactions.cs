using DAOLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOLib.Models
{
    public class Transactions : ModelBase
    {
        public virtual DateTime? date { set; get; }

        public virtual int? accountid { set; get; }

        public virtual Accounts? account { set; get; }

        public virtual int? personid { set; get; }

        public virtual Persons? person { set; get; }

        public virtual int? tagid { set; get; }

        public virtual Tags? tag { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual Categories? category { set; get; }

        public virtual Decimal? amountIn { set; get; }

        public virtual Decimal? amountOut { set; get; }

        public virtual int? tranferid { set; get; }

        public virtual int? tranferSplitid { set; get; }

        public virtual String? memo { set; get; }

        public virtual int? transactionStatusid { set; get; }

        public virtual TransactionsStatus? transactionStatus { set; get; }

        public virtual int? investmentProductsid { set; get; }

        public virtual InvestmentProducts? investmentProducts { set; get; }

        public virtual List<Splits?>? splits { set; get; }

        public virtual Decimal? numShares { set; get; }

        public virtual Decimal? pricesShares { set; get; }

        public virtual bool? investmentCategory { set; get; }

        public virtual String? categoryDescripGrid => (investmentCategory.HasValue && investmentCategory.Value == false) ?
            (numShares > 0 ? "Inversiones:Venta" : "Inversiones:Compra") : category?.description ?? String.Empty;

        public virtual String? personDescripGrid => (investmentCategory.HasValue && investmentCategory.Value == false) ?
            investmentProducts?.description ?? String.Empty : person?.name ?? String.Empty;

        public virtual Decimal? amount => (investmentCategory.HasValue && investmentCategory.Value == false) ? Math.Round((numShares ?? 0) * (pricesShares ?? 0),2) : amountIn - amountOut;

        public virtual Double? orden => Double.Parse(
                    date?.Year.ToString("0000")
                    + date?.Month.ToString("00")
                    + date?.Day.ToString("00")
                    + id.ToString("000000")
                    + (amountIn != 0 ? "1" : "0"));

        
        public virtual Decimal? balance { set; get; }

        internal TransactionsDAO toDAO()
        {
            return new TransactionsDAO()
            {
                id = this.id,
                date = this.date,
                accountid = this.accountid,
                account = null,
                personid = this.personid,
                person = null,
                categoryid = this.categoryid,
                category = null,
                amountIn = this.amountIn,
                amountOut = this.amountOut,
                memo = this.memo,
                investmentCategory = this.investmentCategory,
                investmentProducts = null,
                investmentProductsid = this.investmentProductsid,
                tranferid = this.tranferid,
                tranferSplitid = this.tranferSplitid,
                transactionStatus = null,
                transactionStatusid = this.transactionStatusid,
                numShares = this.numShares,
                pricesShares = this.pricesShares,
                tagid = this.tagid,
                tag = null,
                balance = this.balance
            };
        }


        public static explicit operator Transactions?(TransactionsDAO? v)
        {
            return v == null
                ? null
                : new Transactions()
                {
                    id = v.id,
                    date = v.date,
                    accountid = v.accountid,
                    account = (v.account != null) ? (Accounts?)v.account : null,
                    personid = v.personid,
                    person = (v.person != null) ? (Persons?)v.person : null,
                    categoryid = v.categoryid,
                    category = (v.category != null) ? (Categories?)v.category : null,
                    amountIn = v.amountIn,
                    amountOut = v.amountOut,
                    memo = v.memo,
                    investmentCategory = v.investmentCategory,
                    investmentProducts = (v.investmentProducts != null) ? (InvestmentProducts?)v.investmentProducts : null,
                    investmentProductsid = v.investmentProductsid,
                    tranferid = v.tranferid,
                    tranferSplitid = v.tranferSplitid,
                    transactionStatus = (v.transactionStatus != null) ? (TransactionsStatus?)v.transactionStatus : null,
                    transactionStatusid = v.transactionStatusid,
                    numShares = v.numShares,
                    pricesShares = v.pricesShares,
                    tagid = v.tagid,
                    tag = (v.tag != null) ? (Tags?)v.tag : null,
                    balance = v.balance
                };
        }
    }
}
