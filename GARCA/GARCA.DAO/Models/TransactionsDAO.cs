using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Transactions")]
    public class TransactionsDAO : ModelBaseDAO
    {
        [Column("date")]
        public virtual DateTime? date { set; get; }

        [Column("accountid")]
        public virtual int? accountid { set; get; }

        [Column("account")]
        public virtual AccountsDAO? account { set; get; }

        [Column("personid")]
        public virtual int? personid { set; get; }

        [Column("person")]
        public virtual PersonsDAO? person { set; get; }

        [Column("tagid")]
        public virtual int? tagid { set; get; }

        [Column("tag")]
        public virtual TagsDAO? tag { set; get; }

        [Column("categoryid")]
        public virtual int? categoryid { set; get; }

        [Column("category")]
        public virtual CategoriesDAO? category { set; get; }

        [Column("amountIn")]
        public virtual Decimal? amountIn { set; get; }

        [Column("amountOut")]
        public virtual Decimal? amountOut { set; get; }

        [Column("tranferid")]
        public virtual int? tranferid { set; get; }

        [Column("tranferSplitid")]
        public virtual int? tranferSplitid { set; get; }

        [Column("memo")]
        public virtual String? memo { set; get; }

        [Column("transactionStatusid")]
        public virtual int? transactionStatusid { set; get; }

        [Column("transactionStatus")]
        public virtual TransactionsStatusDAO? transactionStatus { set; get; }

        [Column("investmentProductsid")]
        public virtual int? investmentProductsid { set; get; }

        [Column("investmentProducts")]
        public virtual InvestmentProductsDAO? investmentProducts { set; get; }

        [Column("splits")]
        public virtual HashSet<SplitsDAO>? splits { set; get; }

        [Column("numShares")]
        public virtual Decimal? numShares { set; get; }

        [Column("pricesShares")]
        public virtual Decimal? pricesShares { set; get; }

        [Column("investmentCategory")]
        public virtual bool? investmentCategory { set; get; }

        [Column("balance")]
        public virtual Decimal? balance { set; get; }

        [Column("orden")]
        public virtual Double? orden { set; get; }
    }
}
