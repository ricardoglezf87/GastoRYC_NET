using System;

namespace BBDDLib.Models
{
    public class SplitsReminders
    {
        public virtual int id { set; get; }

        public virtual int? transactionid { set; get; }

        public virtual TransactionsReminders? transaction { set; get; }

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
