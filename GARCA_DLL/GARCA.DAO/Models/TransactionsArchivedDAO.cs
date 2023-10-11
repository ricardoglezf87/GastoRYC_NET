using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("TransactionsArchived")]
    public class TransactionsArchivedDao : ModelBaseDao
    {
        [Column("idOriginal")]
        public virtual int? IdOriginal { get; set; }

        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("accountid")]
        public virtual int? Accountid { set; get; }

        [Column("account")]
        public virtual AccountsDao? Account { set; get; }

        [Column("personid")]
        public virtual int? Personid { set; get; }

        [Column("person")]
        public virtual PersonsDao? Person { set; get; }

        [Column("tagid")]
        public virtual int? Tagid { set; get; }

        [Column("tag")]
        public virtual TagsDao? Tag { set; get; }

        [Column("categoryid")]
        public virtual int? Categoryid { set; get; }

        [Column("category")]
        public virtual CategoriesDao? Category { set; get; }

        [Column("amountIn")]
        public virtual Decimal? AmountIn { set; get; }

        [Column("amountOut")]
        public virtual Decimal? AmountOut { set; get; }

        [Column("tranferid")]
        public virtual int? Tranferid { set; get; }

        [Column("tranferSplitid")]
        public virtual int? TranferSplitid { set; get; }

        [Column("memo")]
        public virtual String? Memo { set; get; }

        [Column("transactionStatusid")]
        public virtual int? TransactionStatusid { set; get; }

        [Column("transactionStatus")]
        public virtual TransactionsStatusDao? TransactionStatus { set; get; }

        [Column("investmentProductsid")]
        public virtual int? InvestmentProductsid { set; get; }

        [Column("investmentProducts")]
        public virtual InvestmentProductsDao? InvestmentProducts { set; get; }

        [Column("splits")]
        public virtual HashSet<SplitsArchivedDao>? Splits { set; get; }

        [Column("numShares")]
        public virtual Decimal? NumShares { set; get; }

        [Column("pricesShares")]
        public virtual Decimal? PricesShares { set; get; }

        [Column("investmentCategory")]
        public virtual bool? InvestmentCategory { set; get; }

        [Column("balance")]
        public virtual Decimal? Balance { set; get; }

        [Column("orden")]
        public virtual Double? Orden { set; get; }
    }
}
