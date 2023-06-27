using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("SplitsReminders")]
    public class SplitsRemindersDAO : ModelBaseDAO
    {
        public virtual int? transactionid { set; get; }

        public virtual TransactionsRemindersDAO? transaction { set; get; }

        public virtual int? tagid { set; get; }

        public virtual TagsDAO? tag { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual CategoriesDAO? category { set; get; }

        public virtual Decimal? amountIn { set; get; }

        public virtual Decimal? amountOut { set; get; }

        public virtual String? memo { set; get; }

        public virtual int? tranferid { set; get; }

    }
}
