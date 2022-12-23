using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBDDLib.Models
{
    public class SplitsReminder
    {
        public virtual int id { set; get; }

        public virtual int? transactionid { set; get; }

        public virtual TransactionsReminder? transaction { set; get; }

        public virtual int? tagid { set; get; }

        public virtual Tags? tag { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual Categories? category { set; get; }

        public virtual Decimal? amountIn { set; get; }

        public virtual Decimal? amountOut { set; get; }        

        public virtual String? memo { set; get; }

        public virtual int? tranferid { set; get; }

    }
}
