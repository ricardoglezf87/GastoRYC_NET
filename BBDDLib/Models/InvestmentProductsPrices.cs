using System;
using System.ComponentModel.DataAnnotations;

namespace BBDDLib.Models
{
    public class InvestmentProductsPrices
    {
        [Key]
        public virtual int id { set; get; }
        public virtual DateTime? date { set; get; }
        public virtual int? invesmentProductid { set; get; }
        public virtual InvestmentProducts? investmentProduct { set; get; }
        public virtual Decimal? prices { set; get; }
    }
}
