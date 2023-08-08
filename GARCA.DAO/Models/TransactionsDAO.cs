using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Transactions")]
    public class TransactionsDAO : ModelBaseDAO
    {
        public virtual DateTime? date { set; get; }
        public virtual int? accountid { set; get; }
        public virtual AccountsDAO? account { set; get; }
        public virtual int? personid { set; get; }
        public virtual PersonsDAO? person { set; get; }
        public virtual int? tagid { set; get; }
        public virtual TagsDAO? tag { set; get; }
        public virtual int? categoryid { set; get; }
        public virtual CategoriesDAO? category { set; get; }
        public virtual Decimal? amountIn { set; get; }
        public virtual Decimal? amountOut { set; get; }
        public virtual int? tranferid { set; get; }
        public virtual int? tranferSplitid { set; get; }
        public virtual String? memo { set; get; }
        public virtual int? transactionStatusid { set; get; }
        public virtual TransactionsStatusDAO? transactionStatus { set; get; }
        public virtual int? investmentProductsid { set; get; }
        public virtual InvestmentProductsDAO? investmentProducts { set; get; }
        public virtual List<SplitsDAO>? splits { set; get; }
        public virtual Decimal? numShares { set; get; }
        public virtual Decimal? pricesShares { set; get; }
        public virtual bool? investmentCategory { set; get; }
        public virtual Decimal? balance { set; get; }
        public virtual Double? orden { set; get; }
    }
}
