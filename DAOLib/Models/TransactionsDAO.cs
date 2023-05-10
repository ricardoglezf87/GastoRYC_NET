using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("Transactions")]
    public class TransactionsDAO
    {
        public virtual int id { set; get; }
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

        [NotMapped]
        public virtual Decimal? amount
        {
            get
            {
                return ((investmentCategory.HasValue && investmentCategory.Value == false) ? (numShares * pricesShares) : amountIn - amountOut);
            }
        }

        [NotMapped]
        public virtual Double? orden
        {
            get
            {
                return Double.Parse(
                    date?.Year.ToString("0000")
                    + date?.Month.ToString("00")
                    + date?.Day.ToString("00")
                    + id.ToString("000000")
                    + (amountIn != 0 ? "1" : "0"));
            }
        }

        [NotMapped]
        public virtual Decimal? balance { set; get; }

    }
}
