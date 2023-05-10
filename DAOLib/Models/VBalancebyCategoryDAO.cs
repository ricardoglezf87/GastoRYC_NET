using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("VBalancebyCategory")]
    public class VBalancebyCategoryDAO
    {
        public virtual int? year { set; get; }
        public virtual int? month { set; get; }
        public virtual int? categoriesTypesid { set; get; }
        public virtual int? categoryid { set; get; }
        public virtual string? category { set; get; }
        public virtual Decimal? amount { set; get; }

        [NotMapped]
        public virtual Decimal? neg_amount { get { return -amount; } }

    }
}
