using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("VPortfolio")]
    public class VPortfolioDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }
        public virtual String? symbol { set; get; }
        public virtual DateTime? date { set; get; }
        public virtual Decimal? prices { set; get; }
        public virtual Decimal? numShares { set; get; }
        public virtual Decimal? costShares { set; get; }
    }
}
