using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBDDLib.Models
{
    public class Transactions
    {
        public virtual int id { set; get; }

        public virtual DateTime? date { set; get; }

        public virtual int? accountid { set; get; }

        public virtual Accounts? account { set; get; }

        public virtual int? personid { set; get; }

        public virtual Persons? person { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual Categories? category { set; get; }

        public virtual Decimal? amountIn { set; get; }

        public virtual Decimal? amountOut { set; get; }

        public virtual int? transactionStatusid { set; get; }

        public virtual TransactionsStatus? transactionStatus { set; get; }

        [NotMapped]
        public virtual Decimal? amount { get { return amountIn - amountOut; } }

        [NotMapped]
        public virtual Double? orden { 
            get {
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
